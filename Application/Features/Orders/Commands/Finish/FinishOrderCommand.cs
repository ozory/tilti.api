using Application.Features.Orders.Contracts;
using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Orders.Commands.FinishOrder;

/// <summary>
/// Command to finish an order after ride completion.
/// This triggers the transfer of funds from Tilt wallet to driver.
/// </summary>
public sealed record FinishOrderCommand(
    long OrderId,
    long UserId
) : ICommand<Result<OrderResponse>>;