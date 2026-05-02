using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Subscriptions.Commands.ActivateDriverSubscription;

/// <summary>
/// Command to activate driver subscription after payment confirmation.
/// Pass subscriptionId (internal ID from externalReference) or asaasPaymentId.
/// Pass dueDate to update the subscription due date on renewal.
/// </summary>
public sealed record ActivateDriverSubscriptionCommand
(
    long subscriptionId,
    string asaasPaymentId,
    DateTime? dueDate = null
) : ICommand<Result<bool>>;