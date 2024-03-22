using Application.Features.Subscriptions.Contracts;
using Application.Shared.Abstractions;

namespace Application.Features.Subscriptions.Commands.Update;

public sealed record UpdateSubscriptionCommand
(
    long SubscriptionId,
    long PlanId,
    DateTime DueDate,
    ushort Status
) : ICommand<SubscriptionResponse>;
