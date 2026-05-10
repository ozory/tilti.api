using Application.Features.Orders.Contracts;
using Application.Features.Payments.Commands.CreatePassengerPayment;
using Domain.Features.Payments.Contracts;
using Application.Features.Users.Commands.CreateUser;
using Application.Shared.Abstractions;
using Domain.Features.Orders.Entities;
using Domain.Orders.Enums;
using Domain.Features.Orders.Events;
using Domain.Shared.Abstractions;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Commands.CreateOrder;

/// <summary>
/// CreateOrderCommandHandler class.
/// </summary>
public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, OrderResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateOrderCommandHandler> _logger;
    private readonly IValidator<CreateOrderCommand> _validator;
    private readonly IPassengerPaymentService _paymentService;
    private readonly string className = nameof(CreateOrderCommandHandler);

    public CreateOrderCommandHandler(
        ILogger<CreateOrderCommandHandler> logger,
        IValidator<CreateOrderCommand> validator,
        IUnitOfWork unitOfWork,
        IPassengerPaymentService paymentService)
    {
        _logger = logger;
        _validator = validator;
        _unitOfWork = unitOfWork;
        _paymentService = paymentService;
    }

    /// <summary>
    /// Handles the creation of an order.
    /// </summary>
    public async Task<Result<OrderResponse>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{className}] Creating an Order {UserId}, PaymentId: {PaymentId}",
            className, request.UserId, request.AsaasPaymentId);

        try
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

            // Validate payment exists and is approved
            if (string.IsNullOrEmpty(request.AsaasPaymentId))
                return Result.Fail("AsaasPaymentId is required for order creation");

            var paymentApproved = await _paymentService.CheckPaymentStatusAsync(request.AsaasPaymentId, cancellationToken);
            if (!paymentApproved)
                return Result.Fail("Payment has not been approved. Cannot create order.");

            var userValidate = await CreateUserCommandValidator.ValidateUser(_unitOfWork.UserRepository, request.UserId);
            if (userValidate.IsFailed) return Result.Fail(userValidate.Errors);

            var openedOrder = await _unitOfWork.OrderRepository.GetOpenedOrdersByUser(request.UserId);
            if (openedOrder.Any()) return Result.Fail("Usuário já possui uma ordem aberta");

            var user = userValidate.Value;
            var order = Order.Create(null, user, request.RequestedTime, request.Addresses, DateTime.Now);

            order.SetAmount(request.Amount);
            order.SetDistanceInKM(request.DistanceInKM);
            order.SetDurationInSeconds(request.DurationInSeconds);
            order.SetPaymentId(request.AsaasPaymentId);

            // Set status to ReadyToAccept (payment confirmed)
            order.SetStatus(OrderStatus.ReadyToAccept);

            // Save order
            var savedOrder = await _unitOfWork.OrderRepository.SaveAsync(order);
            savedOrder.AddDomainEvent((OrderCreatedDomainEvent)savedOrder);

            // Update payment with OrderId
            var payment = await _unitOfWork.PaymentRepository.GetByAsaasPaymentId(request.AsaasPaymentId);
            if (payment != null)
            {
                payment.SetOrderId(savedOrder.Id);
                payment.ApprovePayment();
                await _unitOfWork.PaymentRepository.UpdateAsync(payment);
            }

            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("[{className}] Order Created {savedOrderId} with Status: {Status}",
                className, savedOrder.Id, savedOrder.Status);
            return Result.Ok((OrderResponse)savedOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error creating Order :{request} Error: {ex}", className, request, ex);
            throw;
        }
    }
}
