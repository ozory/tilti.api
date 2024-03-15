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

    IConnection GetConnectionFactory();

    IModel StartNewChannel(string queueName);

    void Consume<T>(IModel sharedChannel, Action<T> action) where T : IDomainEvent;

    void PublishAsync<T>(
        T @event,
        string? exchangeName,
        string? exchangeType,
        string? routingKey,
        string queueName) where T : IDomainEvent;
}
