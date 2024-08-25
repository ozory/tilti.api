using Domain.Enums;
using Domain.Features.Orders.Entities;
using Domain.Features.Orders.Enums;
using Domain.Features.Orders.Events;
using Domain.Shared.Abstractions;
using Domain.Shared.ValueObjects;
using DomainUser = Domain.Features.Users.Entities.User;

namespace Application.Features.Orders.Contracts;

public record OrderResponse
(
    long Id,
    long? DriverId,
    OrderType OrderType,

    OrderUserResponse User,

    string Status,
    List<AddressResponse> Addresses,
    decimal Amount,
    DateTime Created,

    DateTime? RequestedTime,
    DateTime? AcceptanceTime,
    DateTime? CompletionTime,
    DateTime? CancelationTime,
    int DistanceInKM,
    int DurationInSeconds,
    string? Notes
) : IGeoData
{
    public long UserId { get; set; }
    public Location Location { get; set; } = null!;

    public static explicit operator OrderResponse(Order order)
    {
        var orderResponse = new OrderResponse(
                  order.Id,

                  order.Driver?.Id,
                    order.Type,
                  new OrderUserResponse(
                    order.User!.Id,
                    order.User.Name.Value!,
                    order.User.Email.Value!,
                    order.User.Document.Value!,
                    order.User.Photo),

                  order.Status.ToString(),
                  order.Addresses.Select(x => (AddressResponse)x).ToList(),
                  order.Amount.Value,
                  order.CreatedAt,
                  order.RequestedTime,
                  order.AcceptanceTime,
                  order.CompletionTime,
                  order.CancelationTime,
                  order.DistanceInKM,
                  order.DurationInSeconds,
                  order.Notes);

        orderResponse.UserId = order.User.Id;
        orderResponse.Location = order.Location ?? new Location(order.Point.Y, order.Point.X);

        return orderResponse;
    }

    public static explicit operator OrderResponse(OrderCreatedDomainEvent order)
    {
        var orderResponse = new OrderResponse(
                  order.Id,
                  order.DriverId,
                    order.Type,
                  new OrderUserResponse(
                     order.UserId,
                     order.UserName,
                     order.UserEmail!,
                     order.UserDocument!,
                     order.UserPhoto),

                  order.Status.ToString(),
                  order.Addresses.Select(x => (AddressResponse)x).ToList(),
                  order.Amount,
                  order.Created,
                  order.RequestedTime,
                  order.AcceptanceTime,
                  order.CompletionTime,
                  order.CancelationTime,
                  order.DistanceInKM,
                  order.DurationInSeconds,
                  order.Notes);

        orderResponse.UserId = order.UserId;
        orderResponse.Location = order.Location;

        return orderResponse;
    }

    public static explicit operator Order(OrderResponse orderCreated)
    {
        var user = DomainUser.Create(
            orderCreated.UserId,
            orderCreated.User.Name,
            orderCreated.User.Email,
            "",
            null,
            null);

        user.SetPhoto(orderCreated.User.Photo);

        var order = Order.Create(
            orderCreated.Id,
            user,
            orderCreated.RequestedTime!.Value,
            orderCreated.Addresses.Select(x => (Domain.ValueObjects.Address)x).ToList(),
            orderCreated.Created,
            orderCreated.OrderType,
            orderCreated.Notes);

        order.SetAmount(orderCreated.Amount);
        order.SetStatus((OrderStatus)Enum.Parse(typeof(OrderStatus), orderCreated.Status));
        order.SetDistanceInKM(orderCreated.DistanceInKM);
        order.SetDurationInSeconds(orderCreated.DurationInSeconds);
        order.SetLocation(orderCreated.Location.Latitude, orderCreated.Location.Longitude);

        return order;
    }
}
