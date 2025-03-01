using Application.Shared.Abstractions;
using Domain.Features.Users.Events;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Events;

public class UserPasswordUpdatedEventHandler : INotificationHandler<UserCreatedDomainEvent>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserCreatedEventHandler> _logger;

    private readonly string _exchangeName = null!;
    private readonly string _routingKey = null!;
    private readonly string _queueName = null!;
    private readonly string _exchangeType = null!;
    private readonly string className = nameof(UserPasswordUpdatedEventHandler)!;


    public UserPasswordUpdatedEventHandler(
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
        _logger.LogInformation("[{class}] Publicando evento de password atualizado {email}", className, notification.Email);

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
            _logger.LogError("[{class}] Erro ao Publicar evento de password atualizado {email}: {error}", className, notification.Email, ex.Message);
            throw;
        }
    }
}
