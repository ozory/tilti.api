using Application.Features.Orders.Contracts;
using Application.Features.Users.Commands.CreateUser;
using Application.Shared.Abstractions;
using Domain.Features.Orders.Entities;
using Domain.Features.Orders.Events;
using Domain.Orders.Enums;
using Domain.Shared.Abstractions;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Commands.FinishOrder;

/// <summary>
/// Handler for finishing an order after ride completion.
/// This triggers the transfer of funds from Tilt wallet to driver.
/// </summary>
public class FinishOrderCommandHandler : ICommandHandler<FinishOrderCommand, OrderResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FinishOrderCommandHandler> _logger;
    private readonly IValidator<FinishOrderCommand> _validator;
    private readonly string _className = nameof(FinishOrderCommandHandler);

    public FinishOrderCommandHandler(
        ILogger<FinishOrderCommandHandler> logger,
        IValidator<FinishOrderCommand> validator,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles the finishing of an order.
    /// </summary>
    public async Task<Result<OrderResponse>> Handle(
        FinishOrderCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{ClassName}] Finishing order {OrderId}", _className, request.OrderId);

        try
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

            // Get the order
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(request.OrderId);
            if (order == null)
                return Result.Fail("Order not found");

            // Validate order can be finished
            if (order.Status != OrderStatus.InTransit)
                return Result.Fail($"Order cannot be finished from status {order.Status}. Expected InTransit.");

            // Validate driver exists
            if (order.DriverId == null)
                return Result.Fail("Order has no driver assigned. Cannot finish.");

            var driverId = order.DriverId.Value;

            // Validate user is the passenger
            if (order.UserId != request.UserId)
                return Result.Fail("User is not the passenger of this order.");

            // Set completion time and status
            order.SetCompletionTime(DateTime.Now);
            order.SetStatus(OrderStatus.Finished);

            // Save order
            var savedOrder = await _unitOfWork.OrderRepository.UpdateAsync(order);

            // Add domain event for fund transfer
            savedOrder.AddDomainEvent(OrderFinishedDomainEvent.Create(
                savedOrder.Id,
                savedOrder.Amount.Value,
                savedOrder.UserId,
                driverId
            ));

            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("[{ClassName}] Order {OrderId} finished successfully", _className, savedOrder.Id);
            return Result.Ok((OrderResponse)savedOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError("[{ClassName}] Error finishing order {OrderId}: {Exception}",
                _className, request.OrderId, ex);
            return Result.Fail($"Error finishing order: {ex.Message}");
        }
    }
}