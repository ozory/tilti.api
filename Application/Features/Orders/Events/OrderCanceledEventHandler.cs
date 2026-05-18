using Application.Shared.Abstractions;
using Domain.Features.Orders.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Events;

/// <summary>
/// Handles <see cref="OrderCanceledPaymentRefundDomainEvent"/> and forwards it to the
/// messaging infrastructure. The event now serves both as the domain event and the
/// integration message, eliminating the previous duplicate `OrderCanceledDomainEvent`.
/// This mirrors the pattern used for <c>OrderCreatedEventHandler</c> and
/// <c>OrderFinishedEventHandler</c>, keeping the command handler free of direct
/// messaging concerns.
/// </summary>
public class OrderCanceledEventHandler : INotificationHandler<OrderCanceledPaymentRefundDomainEvent>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OrderCanceledEventHandler> _logger;

    private readonly string _exchangeName = null!;
    private readonly string _routingKey = null!;
    private readonly string _queueName = null!;
    private readonly string _exchangeType = null!;
    private readonly string _className = nameof(OrderCanceledEventHandler);

    public OrderCanceledEventHandler(
        IConfiguration configuration,
        ILogger<OrderCanceledEventHandler> logger,
        IMessageRepository messageRepository)
    {
        _configuration = configuration;
        _logger = logger;
        _messageRepository = messageRepository;

        // Re‑use the same configuration keys used by the previous implementation.
        _exchangeName = _configuration["Infrastructure:OrderCanceledPaymentRefundMessages:exchange"] ?? string.Empty;
        _exchangeType = _configuration["Infrastructure:OrderCanceledPaymentRefundMessages:exchangeType"] ??
                        _configuration["Infrastructure:OrderCanceledPaymentRefundMessages:exchanteType"] ?? "topic";
        _queueName = _configuration["Infrastructure:OrderCanceledPaymentRefundMessages:queue"] ?? string.Empty;
        _routingKey = _configuration["Infrastructure:OrderCanceledPaymentRefundMessages:routingKey"] ?? string.Empty;
    }

    public async Task Handle(OrderCanceledPaymentRefundDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{ClassName}] Publishing OrderCanceledPaymentRefund for Order {OrderId}", _className, notification.OrderId);

        var refundEvent = new OrderCanceledPaymentRefundDomainEvent(
            notification.OrderId,
            notification.UserId,
            notification.Amount,
            notification.Reason,
            notification.Description,
            notification.CancellationTime);

        try
        {
            await _messageRepository.PublishAsync(
                refundEvent,
                _exchangeName,
                _exchangeType,
                _routingKey,
                _queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{ClassName}] Error publishing OrderCanceledPaymentRefund for Order {OrderId}", _className, notification.OrderId);
            throw;
        }
    }
}
