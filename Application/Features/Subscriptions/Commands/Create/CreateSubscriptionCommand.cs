using Application.Features.Subscriptions.Contracts;
using Application.Shared.Abstractions;
using FluentResults;
using DomainSubscription = Domain.Features.Subscriptions.Entities.Subscription;

namespace Application.Features.Subscriptions.Commands.Create;

public sealed record CreateSubscriptionCommand
(
    long userId,
    long planId
) : ICommand<SubscriptionResponse>;
