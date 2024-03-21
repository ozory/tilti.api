using Application.Shared.Abstractions;
using Domain.Features.Orders.Events;
using Domain.Features.Users.Events;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Events;

public class OrderCreatedEventHandler : INotificationHandler<OrderCreatedDomainEvent>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserCreatedEventHandler> _logger;
    private readonly ICacheRepository _cacheRepository;

    private readonly string _exchangeName = null!;
    private readonly string _routingKey = null!;
    private readonly string _queueName = null!;
    private readonly string _exchangeType = null!;

    public OrderCreatedEventHandler(
        IConfiguration configuration,
        ILogger<UserCreatedEventHandler> logger,
        IMessageRepository messageRepository,
        ICacheRepository cacheRepository)
    {
        _configuration = configuration;
        _logger = logger;
        _messageRepository = messageRepository;

        _exchangeName = _configuration["Infrastructure:OrderCreatedMessages:exchange"]!;
        _routingKey = _configuration["Infrastructure:OrderCreatedMessages:routingKey"]!;
        _queueName = _configuration["Infrastructure:OrderCreatedMessages:queue"]!;
        _exchangeType = _configuration["Infrastructure:OrderCreatedMessages:exchanteType"]!;
        _cacheRepository = cacheRepository;
    }

    public async Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publicando evento de criação de pedido {id}", notification.Id);

        await _cacheRepository.GeoAdd(
            notification,
            notification.Longitude,
            notification.Latitude,
            $"order-user-{notification.UserId}");

        await Task.CompletedTask;
    }
}
