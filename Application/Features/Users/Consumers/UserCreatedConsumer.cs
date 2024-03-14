using Domain.Features.Users.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Consumers;

public class UserCreatedConsumer(ILogger<UserCreatedConsumer> logger) : IConsumer<UserCreatedDomainEvent>
{
    public async Task Consume(ConsumeContext<UserCreatedDomainEvent> context)
    {
        logger.LogInformation($"Consumindo evento de criação de usuário {context.Message.Email}");

        // Disparando email
        await Task.CompletedTask;
    }
}
