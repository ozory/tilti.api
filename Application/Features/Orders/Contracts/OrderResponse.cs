using DomainOrder = Domain.Features.Orders.Entities.Order;

namespace Application.Features.Orders.Contracts;

public record OrderResponse
(
    long Id,
    long UserId,
    long? DriverId,
    string Status,
    List<AddressResponse> Addresses,
    decimal Amount,
    DateTime Created,

    DateTime? RequestedTime,
    DateTime? AcceptanceTime,
    DateTime? CompletionTime,
    DateTime? CancelationTime,
    int DistanceInKM,
    int DurationInSeconds
)
{
    public static implicit operator OrderResponse(DomainOrder order) =>
        new OrderResponse(
                 order.Id,
                 order.User!.Id,
                 order.Driver?.Id,
                 order.Status.ToString(),
                 order.Addresses.Select(x => (AddressResponse)x).ToList(),
                 order.Amount.Value,
                 order.CreatedAt,
                 order.RequestedTime,
                 order.AcceptanceTime,
                 order.CompletionTime,
                 order.CancelationTime,
                 order.DistanceInKM,
                 order.DurationInSeconds);

}
