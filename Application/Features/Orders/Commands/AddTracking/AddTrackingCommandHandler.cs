using Application.Features.Orders.Contracts;
using Application.Shared.Abstractions;
using Domain.Features.Orders.Entities;
using Domain.Features.Orders.Repository;
using Domain.Orders.Enums;
using Domain.Shared.ValueObjects;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Commands.AddTracking;

public class AddTrackingCommandHandler : ICommandHandler<AddTrackingCommand, OrderResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ITrackingRepository _trackingRepository;
    private readonly ILogger<AddTrackingCommandHandler> _logger;
    private readonly IValidator<AddTrackingCommand> _validator;
    private readonly string className = nameof(AddTrackingCommandHandler);

    public AddTrackingCommandHandler(
        IOrderRepository orderRepository,
        ITrackingRepository trackingRepository,
        ILogger<AddTrackingCommandHandler> logger,
        IValidator<AddTrackingCommand> validator)
    {
        _orderRepository = orderRepository;
        _trackingRepository = trackingRepository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<OrderResponse>> Handle(AddTrackingCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{className}] Adding tracking for order {OrderId}", className, request.OrderId);
        try
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

            var order = await _orderRepository.GetByIdAsync(request.OrderId);
            if (order == null) return Result.Fail("Order not found");

            if (order.Status != OrderStatus.Accepted && order.Status != OrderStatus.InTransit)
                return Result.Fail("Tracking can only be added to orders that are Accepted or InTransit");

            if (order.DriverId != request.DriverId)
                return Result.Fail("Only the assigned driver can add tracking to this order");

            var tracking = Tracking.Create(
                order.Id,
                new Location(request.Latitude, request.Longitude));

            await _trackingRepository.SaveAsync(tracking);

            var updatedOrder = await _orderRepository.GetByIdAsync(request.OrderId);
            return Result.Ok((OrderResponse)updatedOrder!);
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error adding tracking for order {OrderId}: {Error}",
                className, request.OrderId, ex);
            throw;
        }
    }
}