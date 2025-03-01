using Application.Features.Orders.Contracts;
using Application.Shared.Abstractions;
using Domain.ValueObjects;
namespace Application.Features.Orders.Commands.UpdateOrder;

public sealed record UpdateOrderCommand
(
    long UserId,
    long OrderId,
    DateTime requestedTime,
    List<Address> address,

    decimal amount,
    int totalDiscance,
    int totalDuration,
    short orderStatus
) : ICommand<OrderResponse>;


