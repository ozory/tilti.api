using Application.Features.Subscriptions.Contracts;
using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Subscriptions.Commands.CreateDriverSubscription;

/// <summary>
/// Command to create a driver subscription with payment link
/// </summary>
public sealed record CreateDriverSubscriptionCommand
(
    long userId,
    long planId
) : ICommand<Result<DriverSubscriptionResponse>>;