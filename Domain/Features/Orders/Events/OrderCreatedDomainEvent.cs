using Domain.Features.Orders.Entities;
using Domain.Shared.Abstractions;
using Domain.ValueObjects;


using Domain.Features.Users.Entities;
using Domain.Enums;
using Domain.Shared.ValueObjects;

namespace Domain.Features.Orders.Events;

public record OrderCreatedDomainEvent
(
    long Id,
    long UserId,

    string UserName,
    string UserEmail,
    string UserDocument,
    string? UserPhoto,

    long? DriverId,
    string Status,
    List<Address> Addresses,
    decimal Amount,
    DateTime Created,

    double Latitude,
    double Longitude,

    DateTime? RequestedTime,
    DateTime? AcceptanceTime,
    DateTime? CompletionTime,
    DateTime? CancelationTime,
    int DistanceInKM,
    int DurationInSeconds

) : IDomainEvent, IGeoData
{
    Location IGeoData.Location { get => new(this.Latitude, this.Longitude); set { } }

    public static explicit operator OrderCreatedDomainEvent(Order order)
    {
        order.Location ??= new Location(order.Point.Y, order.Point.X);

        return new OrderCreatedDomainEvent(
                 order.Id,
                 order.User!.Id,
                 order.User!.Name!.Value!,
                 order.User!.Email!.Value!,
                 order.User!.Document!.Value!,
                 order.User!.Photo,
                 order.Driver?.Id,
                 order.Status.ToString(),
                 [.. order.Addresses],
                 order.Amount.Value,
                 order.CreatedAt,
                 order.Location!.Latitude,
                 order.Location!.Longitude,
                 order.RequestedTime,
                 order.AcceptanceTime,
                 order.CompletionTime,
                 order.CancelationTime,
                 order.DistanceInKM,
                 order.DurationInSeconds);
    }

    public static explicit operator Order(OrderCreatedDomainEvent orderCreated)
    {
        var user = User.Create(
            orderCreated.UserId,
            orderCreated.UserName,
            orderCreated.UserEmail,
            orderCreated.UserDocument,
            null,
            null);

        if (orderCreated.UserPhoto is not null) user.SetPhoto(orderCreated.UserPhoto);

        var order = Order.Create(
            orderCreated.Id,
            user,
            orderCreated.RequestedTime!.Value,
            [.. orderCreated.Addresses],
            orderCreated.Created);

        order.SetAmount(orderCreated.Amount);
        order.SetStatus((OrderStatus)Enum.Parse(typeof(OrderStatus), orderCreated.Status));
        order.SetDistanceInKM(orderCreated.DistanceInKM);
        order.SetDurationInSeconds(orderCreated.DurationInSeconds);
        order.SetLocation(orderCreated.Latitude, orderCreated.Longitude);

        return order;
    }

}
