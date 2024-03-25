using Application.Features.Orders.Contracts;
using Application.Features.Users.Commands.CreateUser;
using Application.Shared.Abstractions;
using Domain.Features.Orders.Entities;
using Domain.Features.Orders.Events;
using Domain.Features.Orders.Repository;
using Domain.Features.Users.Repository;
using Domain.Shared.Abstractions;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, OrderResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateOrderCommandHandler> _logger;
    private readonly IValidator<CreateOrderCommand> _validator;
    private readonly string className = nameof(CreateOrderCommandHandler);

    public CreateOrderCommandHandler(
        ILogger<CreateOrderCommandHandler> logger,
        IValidator<CreateOrderCommand> validator,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OrderResponse>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{className}] Creating an Order {UserId}", className, request.UserId);

        try
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

            var userValidate = await CreateUserCommandValidator.ValidateUser(_unitOfWork.UserRepository, request.UserId);
            if (userValidate.IsFailed) return Result.Fail(userValidate.Errors);

            var openedOrder = await _unitOfWork.OrderRepository.GetOpenedOrdersByUser(request.UserId);
            if (openedOrder.Any()) return Result.Fail("Usuário já possui uma ordem aberta");

            var user = userValidate.Value;
            var order = Order.Create(null, user, request.requestedTime, request.addresses, DateTime.Now);

            order.SetAmount(request.amount);
            order.SetDistanceInKM(request.distanceInKM);
            order.SetDurationInSeconds(request.durationInSeconds);

            // Save user
            var savedOrder = await _unitOfWork.OrderRepository.SaveAsync(order);
            savedOrder.AddDomainEvent((OrderCreatedDomainEvent)savedOrder);

            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("[{className}] Order Created {savedOrderId}", className, savedOrder.Id);
            return Result.Ok((OrderResponse)savedOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error creating Order :{request} Error: {ex}", className, request, ex);
            throw;
        }

    }
}
