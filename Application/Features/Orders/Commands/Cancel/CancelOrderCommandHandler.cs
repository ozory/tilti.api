using Application.Features.Orders.Contracts;
using Domain.Features.Orders.Events;
using Application.Features.Users.Commands.CreateUser;
using Application.Shared.Abstractions;
using Domain.Enums;
using Domain.Features.Orders.Entities;
using Domain.Features.Orders.Repository;
using Domain.Features.Users.Repository;
using Domain.Orders.Enums;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandler : ICommandHandler<CancelOrderCommand, OrderResponse>
{
    private readonly IOrderRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CancelOrderCommandHandler> _logger;
    private readonly IValidator<CancelOrderCommand> _validator;
    private readonly IMessageRepository _messageRepository;
    private readonly IConfiguration _configuration;
    private readonly string className = nameof(CancelOrderCommandHandler);
    private readonly string _exchangeName = null!;
    private readonly string _exchangeType = null!;
    private readonly string _queueName = null!;
    private readonly string _routingKey = null!;

    public CancelOrderCommandHandler(
        ILogger<CancelOrderCommandHandler> logger,
        IOrderRepository repository,
        IValidator<CancelOrderCommand> validator,
        IUserRepository userRepository,
        IMessageRepository messageRepository,
        IConfiguration configuration)
    {
        _repository = repository;
        _logger = logger;
        _validator = validator;
        _userRepository = userRepository;
        _messageRepository = messageRepository;
        _configuration = configuration;
        _exchangeName = _configuration["Infrastructure:OrderCanceledPaymentRefundMessages:exchange"] ?? string.Empty;
        _exchangeType = _configuration["Infrastructure:OrderCanceledPaymentRefundMessages:exchangeType"] ?? _configuration["Infrastructure:OrderCanceledPaymentRefundMessages:exchanteType"] ?? "topic";
        _queueName = _configuration["Infrastructure:OrderCanceledPaymentRefundMessages:queue"] ?? string.Empty;
        _routingKey = _configuration["Infrastructure:OrderCanceledPaymentRefundMessages:routingKey"] ?? string.Empty;
    }

    public async Task<Result<OrderResponse>> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{className}] Cancelling an Order {Id}", className, request.OrderId);

        try
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

            var userValidate = await CreateUserCommandValidator.ValidateUser(_userRepository, request.UserId);
            if (userValidate.IsFailed) return Result.Fail(userValidate.Errors);

            var openedOrder = await _repository.GetByIdAsync(request.OrderId);
            if (openedOrder is null) return Result.Fail(new List<string> { "Nenhum pedido encontrado" });

            if (openedOrder.Status == OrderStatus.InTransit)
                return Result.Fail("Não é possível cancelar um pedido em andamento");

            if (openedOrder.Status == OrderStatus.Canceled)
                return Result.Fail("Pedido já foi cancelado");

            if (openedOrder.Status == OrderStatus.Finished)
                return Result.Fail("Pedido já foi Finalizado");

            var cancelledBy = request.ParsedCancelledBy ?? CancellationInitiator.User;

            if (cancelledBy == CancellationInitiator.User && openedOrder.UserId != request.UserId)
                return Result.Fail("Usuário não autorizado a cancelar este pedido");

            if (cancelledBy == CancellationInitiator.Driver && openedOrder.DriverId != request.UserId)
                return Result.Fail("Motorista não autorizado a cancelar este pedido");

            openedOrder.SetStatus(OrderStatus.Canceled);
            openedOrder.SetCancelDescription(request.description);
            openedOrder.SetCancelRasons(string.Join(",", request.reason ?? new List<string>()));
            openedOrder.SetCancelledBy(cancelledBy);
            openedOrder.SetCancelationTime(DateTime.UtcNow);

            var canceledOrder = await _repository.UpdateAsync(openedOrder);

            // Publish refund event regardless of who cancelled the order.
            // The consumer will decide whether to apply a penalty based on the CancelledBy flag.
            var refundEvent = new OrderCanceledPaymentRefundDomainEvent(
                request.OrderId,
                request.UserId,
                openedOrder.Amount.Value,
                request.reason ?? new List<string>(),
                request.description ?? string.Empty,
                DateTime.UtcNow);

            await _messageRepository.PublishAsync(
                refundEvent,
                _exchangeName,
                _exchangeType,
                _routingKey,
                _queueName);

            _logger.LogInformation("[{className}] Published OrderCanceledPaymentRefund for Order {Id}", className, request.OrderId);


            _logger.LogInformation("[{className}] Order Canceled {Id}", className, request.OrderId);
            return Result.Ok((OrderResponse)canceledOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error Canceling Order : {request} Error: {ex}", className, request, ex);
            return Result.Fail(ex.Message);
        }

    }
}
