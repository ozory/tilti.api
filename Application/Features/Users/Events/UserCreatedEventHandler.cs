using Domain.Features.Users.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace Application.Features.Users.Events;

public class UserCreatedEventHandler : INotificationHandler<UserCreatedDomainEvent>
{
    private readonly IMessageBus _bus;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserCreatedEventHandler> _logger;

    public UserCreatedEventHandler(IMessageBus bus, IConfiguration configuration, ILogger<UserCreatedEventHandler> logger)
    {
        _bus = bus;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken)
    {


        // ADD Rabbit OWN Facade
        await _bus.PublishAsync(notification);
        _logger.LogInformation($"Publicando evento de criação de usuário {notification.Email}");

        await Task.CompletedTask;
    }
}
