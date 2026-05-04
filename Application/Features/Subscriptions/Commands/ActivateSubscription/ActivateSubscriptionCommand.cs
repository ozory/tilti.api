using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Subscriptions.Commands.ActivateSubscription;

/// <summary>
/// Command to activate subscription after payment confirmation.
/// Pass subscriptionId (internal ID from externalReference) or asaasPaymentId.
/// Pass dueDate to update the subscription due date on renewal.
/// </summary>
public sealed record ActivateSubscriptionCommand
(
    long subscriptionId,
    string asaasPaymentId,
    DateTime? dueDate = null
) : ICommand<Result<bool>>;