using Domain.Features.Users.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Events;

public class UserCreatedEventHandler : INotificationHandler<UserCreatedDomainEvent>
{
    private readonly IBus _bus;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserCreatedEventHandler> _logger;

    public UserCreatedEventHandler(IBus bus, IConfiguration configuration, ILogger<UserCreatedEventHandler> logger)
    {
        _bus = bus;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var sender = await _bus.GetSendEndpoint(new Uri("exchange:user-created-exchange"));
        // var sender = await _bus.GetSendEndpoint(new Uri(_configuration["Infrastructure:Queues:UserCreatedQueue"]!));
        // var newSender = await _bus.GetPublishSendEndpoint<UserCreatedDomainEvent>();

        await sender.Send(notification, cancellationToken);

        _logger.LogInformation($"Publicando evento de criação de usuário {notification.Email}");
    }
}
