using Application.Features.Orders.Contracts;
using Application.Features.Users.Contracts;
using Application.Shared.Abstractions;
using Domain.ValueObjects;
namespace Application.Features.Orders.Commands.PrecifyOrder;

public sealed record PrecifyOrderCommand
(
    long UserId,
    List<Address> address

) : ICommand<OrderResponse>;


