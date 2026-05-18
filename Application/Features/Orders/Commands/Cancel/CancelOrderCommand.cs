using Application.Features.Orders.Contracts;
using Application.Shared.Abstractions;
using Domain.Orders.Enums;
using Domain.ValueObjects;
using FluentResults;

namespace Application.Features.Orders.Commands.CancelOrder;

public sealed record CancelOrderCommand(
    long UserId,
    long OrderId,
    DateTime requestedTime,
    List<string>? reason = null,
    string? description = null,
    string? cancelledBy = null
) : ICommand<Result<OrderResponse>>
{
    public CancellationInitiator? ParsedCancelledBy =>
        cancelledBy == null ? null :
        cancelledBy.Equals("User", StringComparison.OrdinalIgnoreCase) ? CancellationInitiator.User :
        cancelledBy.Equals("Driver", StringComparison.OrdinalIgnoreCase) ? CancellationInitiator.Driver :
        null;
};


