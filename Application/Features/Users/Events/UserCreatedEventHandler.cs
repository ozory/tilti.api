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

    public UserCreatedEventHandler(
        IConfiguration configuration,
        ILogger<UserCreatedEventHandler> logger,
        IMessageRepository messageRepository)
    {
        _configuration = configuration;
        _logger = logger;
        _messageRepository = messageRepository;
    }

    public async Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken)
    {

        _messageRepository.PublishAsync(
            notification,
            "user-created-exchange",
            "topic",
            "user-created",
            "user-created-queue");

        _logger.LogInformation($"Publicando evento de criação de usuário {notification.Email}");

        await Task.CompletedTask;
    }
}
