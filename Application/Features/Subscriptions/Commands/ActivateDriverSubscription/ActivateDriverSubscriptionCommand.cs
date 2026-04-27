using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Subscriptions.Commands.ActivateDriverSubscription;

/// <summary>
/// Command to activate driver subscription after payment confirmation
/// </summary>
public sealed record ActivateDriverSubscriptionCommand
(
    long subscriptionId,
    string asaasPaymentId
) : ICommand<Result<bool>>;