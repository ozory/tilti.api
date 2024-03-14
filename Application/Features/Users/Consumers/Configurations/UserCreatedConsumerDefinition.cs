using MassTransit;

namespace Application.Features.Users.Consumers;

public class UserCreatedConsumerDefinition : ConsumerDefinition<UserCreatedConsumer>
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

        configurator.ConfigureConsumeTopology = false;
        configurator.PrefetchCount = 10;
        configurator.UseMessageRetry(r => r.Interval(5, 1000));
        configurator.UseInMemoryOutbox(context);
    }
}
