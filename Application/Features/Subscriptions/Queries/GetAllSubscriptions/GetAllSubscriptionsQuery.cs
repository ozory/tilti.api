using System.Collections.Immutable;
using Application.Features.Subscriptions.Contracts;
using Application.Shared.Abstractions;

namespace Application.Features.Subscriptions.Queries.GetAllSubscriptions;

public sealed record GetAllSubscriptionsQuery() : IQuery<ImmutableList<SubscriptionResponse>>;