
using Application.Features.Orders.Contracts;
using Application.Shared.Abstractions;
using Domain.ValueObjects;
namespace Application.Features.Orders.Commands.CreateOrder;

/// <summary>
/// Command for creating a new order in the system.
/// </summary>
/// <param name="UserId">The unique identifier of the user placing the order</param>
/// <param name="RequestedTime">The timestamp when the order was requested</param>
/// <param name="Addresses">List of addresses involved in the order, including pickup and delivery locations</param>
/// <param name="Amount">The total monetary amount of the order</param>
/// <param name="DistanceInKM">The total distance of the delivery route in kilometers</param>
/// <param name="DurationInSeconds">The estimated duration of the delivery in seconds</param>
/// <returns>Returns an OrderResponse containing the created order details</returns>
public sealed record CreateOrderCommand
(
    long UserId,
    DateTime RequestedTime,
    List<Address> Addresses,

    decimal Amount,
    int DistanceInKM,
    int DurationInSeconds

) : ICommand<OrderResponse>;


