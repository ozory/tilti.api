using Application.Features.Subscriptions.Contracts;
using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Subscriptions.Queries.GetSubscription;

/// <summary>
/// Query to get subscription by user ID
/// </summary>
public sealed record GetSubscriptionQuery(long userId) : IQuery<Result<SubscriptionResponse>>;