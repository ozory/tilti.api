using Domain.Features.Users.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Consumers;

public class UserCreatedConsumer(ILogger<UserCreatedConsumer> logger)
{
    public async Task Handle(UserCreatedDomainEvent userCreated)
    {
        logger.LogInformation($"Consumindo evento de criação de usuário {userCreated.Email}");

        //await context.NotifyFaulted<UserCreatedDomainEvent>(context, TimeSpan.FromSeconds(-1), "", new Exception(""));
        // throw new Exception("ERRO PRIMARIO");
        // Disparando email
        await Task.CompletedTask;
    }
}
