using Domain.Enums;
using Infrastructure.Data.Postgreesql.Features.Orders.Entities;
using Infrastructure.Data.Postgreesql.Features.Users.Maps;
using DomainOrder = Domain.Features.Orders.Entities.Order;
using DomainAddress = Domain.ValueObjects.Address;
using Domain.Features.Users.Entities;

namespace Infrastructure.Data.Postgreesql.Features.Subscriptions.Maps;

public static class OrderMap
{
    internal static Order ToPersistenceOrder(this DomainOrder order)
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

        persistenceOrder.Addresses.AddRange(order.ToPersistenceAddress());

        return persistenceOrder;
    }

    internal static ICollection<Address> ToPersistenceAddress(this DomainOrder order)
    {
        return order.Addresses.Select(x => new Address()
        {
            AddressType = (ushort)x.AddressType,
            City = x.City,
            Street = x.Street,
            Number = x.Number,
            Complment = x.Complment,
            ZipCode = x.ZipCode,
            Neighborhood = x.Neighborhood,
            Latitude = x.Latitude,
            Longitude = x.Longitude,
        }).ToList();
    }

    internal static DomainOrder ToDomainOrder(this Order order)
    {
        var domainOrder = DomainOrder.Create(
            order.Id,
            order.User?.ToDomainUser(),
            order.RequestedTime!.Value,
            order.ToDomainAddress().ToList(),
            order.Created
        );

        domainOrder.SetDriver(order.Driver?.ToDomainUser());
        domainOrder.SetStatus((OrderStatus)order.Status);
        domainOrder.SetUpdatedAt(order.Updated);
        domainOrder.SetAmount(order.Amount);

        domainOrder.SetAcceptanceTime(order.AcceptanceTime);
        domainOrder.SetCompletionTime(order.CompletionTime);

        if (order.CancelationTime is not null) domainOrder.SetCancelationTime(order.CancelationTime);
        if (domainOrder.User == null) domainOrder.SetUser(User.Create(order.UserId));

        return domainOrder;
    }

    internal static ICollection<DomainAddress> ToDomainAddress(this Order order)
    {
        return order.Addresses.Select(x => new DomainAddress()
        {
            AddressType = (AddressType)x.AddressType,
            City = x.City,
            Street = x.Street,
            Number = x.Number,
            Complment = x.Complment,
            ZipCode = x.ZipCode,
            Neighborhood = x.Neighborhood,
            Latitude = x.Latitude,
            Longitude = x.Longitude,
        }).ToList();
    }
}
