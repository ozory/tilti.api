using Application.Features.Orders.Contracts;
using Application.Features.Users.Contracts;
using Application.Shared.Abstractions;
using Domain.ValueObjects;
namespace Application.Features.Orders.Commands.CreateOrder;

public sealed record CreateOrderCommand
(
    long UserId,
    DateTime requestedTime,
    List<Address> address

) : ICommand<OrderResponse>;


