using Application.Features.Subscriptions.Contracts;
using Application.Shared.Abstractions;

namespace Application.Features.Subscriptions.Queries.GetSubscriptionByUser;

public sealed record GetSubscriptionByUserQuery(long userId) : IQuery<SubscriptionResponse>;