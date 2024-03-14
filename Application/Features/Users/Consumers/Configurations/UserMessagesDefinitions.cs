using Domain.Features.Users.Events;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using RabbitMQ.Client;
using Wolverine;
using Wolverine.RabbitMQ;
using Wolverine.RabbitMQ.Internal;

namespace Application.Features.Users.Consumers;

public static class UserCreatedConsumerDefinition
{
    public static RabbitMqTransportExpression AddUserMessageDefinitions(
        this RabbitMqTransportExpression config,
        WebApplicationBuilder builder,
        WolverineOptions opts)
    {

        var userCreatedExchange = builder.Configuration["Infrastructure:UserCreatedMessages:exchange"]!;
        var userCreatedQueue = builder.Configuration["Infrastructure:UserCreatedMessages:queue"]!;
        var userCreatedRoutingKey = builder.Configuration["Infrastructure:UserCreatedMessages:routingKey"]!;

        config.DeclareExchange(userCreatedExchange, ex =>
                {
                    ex.ExchangeType = Wolverine.RabbitMQ.ExchangeType.Direct;
                    ex.BindQueue(userCreatedQueue, userCreatedRoutingKey);
                    ex.IsDurable = true;
                })
            .AutoProvision();

        opts.PublishMessage<UserCreatedDomainEvent>()
            .ToRabbitRoutingKey(userCreatedExchange, userCreatedRoutingKey)
            .UseDurableOutbox();

        opts.ListenToRabbitQueue(userCreatedQueue)
               .PreFetchCount(100)
               .ListenerCount(5)
               .UseDurableInbox();

        return config;
    }
}
