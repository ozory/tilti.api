using Application.Features.Orders.Contracts;
using Application.Shared.Abstractions;
using Domain.ValueObjects;
namespace Application.Features.Orders.Commands.CancelOrder;

public sealed record CancelOrderCommand
(
    long UserId,
    long OrderId,
    DateTime requestedTime,
    List<string> reason,
    string description

) : ICommand<OrderResponse>;


