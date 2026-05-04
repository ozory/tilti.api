using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Subscriptions.Commands.CancelSubscription;

/// <summary>
/// Command to cancel subscription
/// </summary>
public sealed record CancelSubscriptionCommand
(
    long subscriptionId,
    string? reason = null
) : ICommand<Result<bool>>;