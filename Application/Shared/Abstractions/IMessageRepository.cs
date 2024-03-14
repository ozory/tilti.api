using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Shared.Abstractions;

namespace Application.Shared.Abstractions;

public interface IMessageRepository
{
    void PublishAsync<T>(
        T @event,
        string? exchangeName,
        string? exchangeType,
        string? routingKey,
        string queueName)
        where T : IDomainEvent;
}
