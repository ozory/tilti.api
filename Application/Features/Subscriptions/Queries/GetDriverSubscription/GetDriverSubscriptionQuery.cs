using Application.Features.Subscriptions.Contracts;
using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Subscriptions.Queries.GetDriverSubscription;

/// <summary>
/// Query to get driver subscription by user ID
/// </summary>
public sealed record GetDriverSubscriptionQuery(long userId) : IQuery<Result<DriverSubscriptionResponse>>;