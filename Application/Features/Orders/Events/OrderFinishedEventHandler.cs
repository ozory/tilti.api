using Application.Shared.Abstractions;
using Domain.Features.Orders.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Events;

/// <summary>
/// Event handler that publishes order finished event to queue.
/// This triggers the transfer of funds from Tilt wallet to driver.
/// </summary>
public class OrderFinishedEventHandler : INotificationHandler<OrderFinishedDomainEvent>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OrderFinishedEventHandler> _logger;

    private readonly string _exchangeName = null!;
    private readonly string _routingKey = null!;
    private readonly string _queueName = null!;
    private readonly string _exchangeType = null!;
    private readonly string _className = nameof(OrderFinishedEventHandler);

    public OrderFinishedEventHandler(
        IConfiguration configuration,
        ILogger<OrderFinishedEventHandler> logger,
        IMessageRepository messageRepository)
    {
        _configuration = configuration;
        _logger = logger;
        _messageRepository = messageRepository;

        _exchangeName = _configuration["Infrastructure:OrderFinishedMessages:exchange"]!;
        _routingKey = _configuration["Infrastructure:OrderFinishedMessages:routingKey"]!;
        _queueName = _configuration["Infrastructure:OrderFinishedMessages:queue"]!;
        _exchangeType = _configuration["Infrastructure:OrderFinishedMessages:exchangeType"]!;
    }

    public async Task Handle(OrderFinishedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "[{ClassName}] Publishing order finished event for Order {OrderId}, Amount: {Amount}, Driver: {DriverId}",
            _className, notification.OrderId, notification.Amount, notification.DriverId);

        try
        {
            await _messageRepository.PublishAsync(
                notification,
                _exchangeName,
                _exchangeType,
                _routingKey,
                _queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "[{ClassName}] Error publishing order finished event for Order {OrderId}: {Exception}",
                _className, notification.OrderId, ex);
            throw;
        }
    }
}