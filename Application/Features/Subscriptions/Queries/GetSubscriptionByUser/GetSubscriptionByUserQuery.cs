using Application.Features.Subscriptions.Contracts;
using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Subscriptions.Queries.GetSubscriptionByUser;

public sealed record GetSubscriptionByUserQuery(long userId) : IQuery<Result<SubscriptionResponse>>;