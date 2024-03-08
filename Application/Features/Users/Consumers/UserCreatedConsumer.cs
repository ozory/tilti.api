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

public class UserCreatedConsumerDefinition :
    ConsumerDefinition<UserCreatedConsumer>
{
    public UserCreatedConsumerDefinition()
    {
        EndpointName = "user-created";
        // limit the number of messages consumed concurrently
        // this applies to the consumer only, not the endpoint
        ConcurrentMessageLimit = 5;
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator configurator,
        IConsumerConfigurator<UserCreatedConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        configurator.UseMessageRetry(r => r.Interval(5, 1000));
        configurator.UseInMemoryOutbox(context);
    }
}
