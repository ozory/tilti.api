using Domain.Features.Orders.Entities;
using Domain.Shared.Abstractions;
using Domain.ValueObjects;


using Domain.Features.Users.Entities;
using Domain.Shared.ValueObjects;
using Domain.Shared.Enums;
using Domain.Orders.Enums;

namespace Domain.Features.Orders.Events;

public record OrderCreatedDomainEvent
(
    long Id,
    OrderType Type,
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
    int DurationInSeconds,
    string? Notes = null

) : IDomainEvent, IGeoData
{
    public Location Location { get => new(this.Latitude, this.Longitude); set { } }

    public long UserId { get; set; }

    public static explicit operator OrderCreatedDomainEvent(Order order)
    {
        order.Location ??= new Location(order.Point.Y, order.Point.X);

        var odrderCreated = new OrderCreatedDomainEvent(
                 order.Id,
                order.Type,
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
                 order.DurationInSeconds,
                 order.Notes);

        odrderCreated.UserId = order.User!.Id;
        return odrderCreated;
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
            orderCreated.Created,
            orderCreated.Type,
            orderCreated.Notes);

        order.SetAmount(orderCreated.Amount);
        order.SetStatus((OrderStatus)Enum.Parse(typeof(OrderStatus), orderCreated.Status));
        order.SetDistanceInKM(orderCreated.DistanceInKM);
        order.SetDurationInSeconds(orderCreated.DurationInSeconds);
        order.SetLocation(orderCreated.Latitude, orderCreated.Longitude);

        return order;
    }

}
