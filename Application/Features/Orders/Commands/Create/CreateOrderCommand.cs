using Application.Features.Orders.Contracts;
using Application.Shared.Abstractions;
using Domain.ValueObjects;
namespace Application.Features.Orders.Commands.CreateOrder;

public sealed record CreateOrderCommand
(
    long UserId,
    DateTime requestedTime,
    List<Address> addresses,

    decimal amount,
    int distanceInKM,
    int durationInSeconds

) : ICommand<OrderResponse>;


