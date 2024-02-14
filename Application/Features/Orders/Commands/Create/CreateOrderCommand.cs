using Application.Features.Orders.Contracts;
using Application.Features.Users.Contracts;
using Application.Shared.Abstractions;
namespace Application.Features.Orders.Commands.CreateOrder;

public sealed record CreateOrderCommand
(
    string Name,
    string Email,
    string Document,
    string Password

) : ICommand<OrderResponse>;


