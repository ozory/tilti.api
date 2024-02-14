using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using DomainOrder = Domain.Features.Orders.Entities.Order;

namespace Application.Features.Orders.Contracts;

public record OrderResponse
(
    long Id,
    long UserId,
    long? DriverId,
    ushort Status,
    decimal Amount,

    DateTime Created,

    DateTime? RequestedTime,
    DateTime? AcceptanceTime,
    DateTime? CompletionTime,
    DateTime? CancelationTime
)
{
    public static implicit operator OrderResponse(DomainOrder order)
       => new OrderResponse(
                order.Id,
                order.User.Id,
                order.Driver?.Id,
                (ushort)order.Status,
                order.Amount.Value,
                order.CreatedAt,

                order.RequestedTime,
                order.AcceptanceTime,
                order.CompletionTime,
                order.CancelationTime);
}
