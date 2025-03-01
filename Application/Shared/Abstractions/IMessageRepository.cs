using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Shared.Abstractions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Application.Shared.Abstractions;

public interface IMessageRepository
{
    IMessageRepository CreateNewInstance();

    Task<IConnection> GetConnectionFactory();

    Task<IChannel> StartNewChannel(string queueName);

    Task Consume<T>(IChannel sharedChannel, Action<T> action) where T : IDomainEvent;

    Task PublishAsync<T>(
        T @event,
        string? exchangeName,
        string? exchangeType,
        string? routingKey,
        string queueName) where T : IDomainEvent;
}
