using Domain.Shared.Abstractions;

namespace Domain.Features.Orders.Events;

/// <summary>
/// Domain event published when an order is finished successfully.
/// This event triggers the transfer of funds from Tilt wallet to driver.
/// </summary>
public record OrderFinishedDomainEvent(
    long OrderId,
    decimal Amount,
    long PassengerId,
    long DriverId,
    DateTime FinishedAt
) : IDomainEvent
{
    public static OrderFinishedDomainEvent Create(long orderId, decimal amount, long passengerId, long driverId)
    {
        return new OrderFinishedDomainEvent(
            orderId,
            amount,
            passengerId,
            driverId,
            DateTime.Now
        );
    }
}