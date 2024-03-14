using Domain.Features.Users.Events;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Consumers;

public class UserCreatedConsumer(ILogger<UserCreatedConsumer> logger)
{
    public async Task Handle(UserCreatedDomainEvent userCreated)
    {
        logger.LogInformation($"Consumindo evento de criação de usuário {userCreated.Email}");

        await Task.CompletedTask;
    }
}
