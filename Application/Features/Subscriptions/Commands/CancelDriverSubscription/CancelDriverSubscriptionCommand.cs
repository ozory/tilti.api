using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Subscriptions.Commands.CancelDriverSubscription;

/// <summary>
/// Command to cancel driver subscription
/// </summary>
public sealed record CancelDriverSubscriptionCommand
(
    long subscriptionId,
    string? reason = null
) : ICommand<Result<bool>>;