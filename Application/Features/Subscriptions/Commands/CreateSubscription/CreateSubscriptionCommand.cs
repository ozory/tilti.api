using Application.Shared.Abstractions;
using FluentResults;
using DomainSubscription = Domain.Features.Subscription.Entities.Subscription;

namespace Application.Features.Subscriptions.Commands.CreateSubscription;

public sealed record CreateSubscriptionCommand
(
    long userId,
    long planid
) : ICommand<DomainSubscription>;
