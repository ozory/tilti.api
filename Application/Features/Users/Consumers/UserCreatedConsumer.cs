using Domain.Features.Users.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Consumers;

public class UserCreatedConsumer(ILogger<UserCreatedConsumer> logger) : IConsumer<UserCreatedDomainEvent>
{
    public async Task Consume(ConsumeContext<UserCreatedDomainEvent> context)
    {
        logger.LogInformation($"Consumindo evento de criação de usuário {context.Message.Email}");

        //await context.NotifyFaulted<UserCreatedDomainEvent>(context, TimeSpan.FromSeconds(-1), "", new Exception(""));
        // throw new Exception("ERRO PRIMARIO");
        // Disparando email
        await Task.CompletedTask;
    }
}
