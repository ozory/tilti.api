using Domain.Features.Orders.Entities;
using Domain.Shared.Abstractions;
using Domain.ValueObjects;
using GeoPoint = NetTopologySuite.Geometries.Point;

namespace Domain.Features.Orders.Events;

public record OrderCreatedDomainEvent
(
    long Id,
    long UserId,
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
) : IDomainEvent
{
    public static explicit operator OrderCreatedDomainEvent(Order order) =>
        new OrderCreatedDomainEvent(
                 order.Id,
                 order.User!.Id,
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
