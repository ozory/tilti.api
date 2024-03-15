using Application.Shared.Abstractions;
using Domain.Features.Users.Events;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Events;

public class UserCreatedEventHandler : INotificationHandler<UserCreatedDomainEvent>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserCreatedEventHandler> _logger;

    private readonly string _exchangeName = null!;
    private readonly string _routingKey = null!;
    private readonly string _queueName = null!;
    private readonly string _exchangeType = null!;

    public UserCreatedEventHandler(
        IConfiguration configuration,
        ILogger<UserCreatedEventHandler> logger,
        IMessageRepository messageRepository)
    {
        _configuration = configuration;
        _logger = logger;
        _messageRepository = messageRepository;

        _exchangeName = _configuration["Infrastructure:UserCreatedMessages:exchange"]!;
        _routingKey = _configuration["Infrastructure:UserCreatedMessages:routingKey"]!;
        _queueName = _configuration["Infrastructure:UserCreatedMessages:queue"]!;
        _exchangeType = _configuration["Infrastructure:UserCreatedMessages:exchanteType"]!;
    }

    public async Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publicando evento de criação de usuário {email}", notification.Email);

        _messageRepository.PublishAsync(
            notification,
            _exchangeName,
            _exchangeType,
            _routingKey,
            _queueName);

        await Task.CompletedTask;
    }
}
