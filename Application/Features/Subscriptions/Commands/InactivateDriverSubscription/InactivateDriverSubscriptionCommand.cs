using Application.Features.Subscriptions.Contracts;
using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Subscriptions.Commands.InactivateDriverSubscription;

/// <summary>
/// Command to inactivate a driver subscription
/// </summary>
public sealed record InactivateDriverSubscriptionCommand(
    string AsaasSubscriptionId
) : ICommand<Result<DriverSubscriptionResponse>>;