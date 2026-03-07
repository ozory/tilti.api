using Application.Features.Orders.Contracts;
using Application.Shared.Abstractions;
using Domain.ValueObjects;
using FluentResults;
namespace Application.Features.Orders.Commands.PrecifyOrder;

public sealed record PrecifyOrderCommand
(
    long UserId,
    List<Address> address

) : ICommand<Result<OrderResponse>>;


