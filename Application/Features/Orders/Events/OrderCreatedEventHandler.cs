using Application.Features.Orders.Contracts;
using Application.Shared.Abstractions;
using Application.Shared.Extensions;
using Domain.Features.Orders.Events;
using Domain.Features.Users.Events;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Events;

public class OrderCreatedEventHandler : INotificationHandler<OrderCreatedDomainEvent>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OrderCreatedDomainEvent> _logger;
    private readonly ICacheRepository _cacheRepository;

    private readonly string _exchangeName = null!;
    private readonly string _routingKey = null!;
    private readonly string _queueName = null!;
    private readonly string _exchangeType = null!;
    private readonly string className = nameof(OrderCreatedEventHandler)!;

    public OrderCreatedEventHandler(
        IConfiguration configuration,
        ILogger<OrderCreatedDomainEvent> logger,
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
        _logger.LogInformation("[{class}] Publicando evento de criação de pedido {id}", className, notification.Id);

        try
        {
            await _cacheRepository.GeoAdd((OrderResponse)notification);
        }
        catch (System.Exception)
        {
            _logger.LogError("[{class}] Erro ao Publicar evento de criação de pedido {id}", className, notification.Id);
            throw;
        }
    }
}
