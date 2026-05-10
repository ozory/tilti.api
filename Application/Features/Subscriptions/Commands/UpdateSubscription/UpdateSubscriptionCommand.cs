using Application.Features.Subscriptions.Contracts;
using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Subscriptions.Commands.UpdateSubscription;

public sealed record UpdateSubscriptionCommand
(
    long SubscriptionId,
    long PlanId,
    DateTime DueDate,
    ushort Status
) : ICommand<Result<SubscriptionResponse>>;
