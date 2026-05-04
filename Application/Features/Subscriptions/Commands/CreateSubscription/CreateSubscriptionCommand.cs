using Application.Features.Subscriptions.Contracts;
using Application.Shared.Abstractions;
using FluentResults;
using Domain.Subscriptions.Enums;

namespace Application.Features.Subscriptions.Commands.CreateSubscription;

/// <summary>
/// Command to create a subscription (driver or passenger)
/// </summary>
public sealed record CreateSubscriptionCommand
(
    long userId,
    long planId,
    SubscriptionType subscriptionType = SubscriptionType.Driver // NEW: Default to Driver for backward compatibility
) : ICommand<Result<SubscriptionResponse>>;