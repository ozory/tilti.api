using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Subscriptions.Commands.DeleteDriverSubscription;

/// <summary>
/// Command to delete a driver subscription
/// </summary>
public sealed record DeleteDriverSubscriptionCommand(
    string AsaasSubscriptionId
) : ICommand<Result>;