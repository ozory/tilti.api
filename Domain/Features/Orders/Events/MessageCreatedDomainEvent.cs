using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Domain.Features.Orders.Entities;
using Domain.Shared.Abstractions;

namespace Domain.Features.Orders.Events;

public record CreateMessageDomainEvent
(
    long OrderId,
    long UserId,
    long DriverId,
    DateTime CreatedAt,
    string Message
) : IDomainEvent
{
    public static explicit operator CreateMessageDomainEvent(
        Message message)
    {
        return new CreateMessageDomainEvent(
            message.OrderId,
            message.SourceUserId,
            message.TargetUserId,
            message.CreatedAt,
            message.Value
        );
    }
}

