using Domain.Enums;
using Domain.Features.Orders.Entities;
using Infrastructure.Data.Postgreesql.Features.Users.Entities;
using DomainOrder = Domain.Features.Orders.Entities.Order;
using DomainUser = Domain.Features.Users.Entities.User;
using DomainAddress = Domain.ValueObjects.Address;
using Infrastructure.Data.Postgreesql.Shared.Abstractions;

namespace Infrastructure.Data.Postgreesql.Features.Orders.Entities;

public class Order : InfrastructureEntity
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long? DriverId { get; set; }
    public ushort Status { get; set; }
    public decimal Amount { get; set; }

    public DateTime Created { get; set; }
    public DateTime Updated { get; set; } = DateTime.Now;

    public DateTime? RequestedTime { get; set; }
    public DateTime? AcceptanceTime { get; set; }
    public DateTime? CompletionTime { get; set; }
    public DateTime? CancelationTime { get; set; }

    public List<Address> Addresses { get; } = [];

    public User User { get; set; } = null!;
    public User? Driver { get; set; }

    public static implicit operator DomainOrder(Order? order)
    {
        if (order is null) return null!;
        var domainOrder = DomainOrder.Create(
            order.Id,
            order.User,
            order.RequestedTime!.Value,
            order.Addresses.Select(address => (DomainAddress)address).ToList(),
            order.Created
        );

        domainOrder.SetDriver(order.Driver);
        domainOrder.SetStatus((OrderStatus)order.Status);
        domainOrder.SetUpdatedAt(order.Updated);
        domainOrder.SetAmount(order.Amount);

        domainOrder.SetAcceptanceTime(order.AcceptanceTime);
        domainOrder.SetCompletionTime(order.CompletionTime);

        if (order.CancelationTime is not null) domainOrder.SetCancelationTime(order.CancelationTime);
        if (domainOrder.User == null) domainOrder.SetUser(DomainUser.Create(order.UserId));

        return domainOrder;
    }

    public static explicit operator Order(DomainOrder order)
    {
        Order persistenceOrder = new()
        {
            Id = order.Id,
            UserId = order.User!.Id,
            DriverId = order.Driver?.Id,
            RequestedTime = order.RequestedTime,
            AcceptanceTime = order.AcceptanceTime,
            CancelationTime = order.CancelationTime,
            CompletionTime = order.CompletionTime,
            Created = order.CreatedAt,
            Status = (ushort)order.Status,
        };

        var address = order.Addresses.Select(address => (Address)address);
        persistenceOrder.Addresses.AddRange(address);

        return persistenceOrder;
    }


}
