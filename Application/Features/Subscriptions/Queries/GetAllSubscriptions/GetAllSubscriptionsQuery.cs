using System.Collections.Immutable;
using Application.Features.Subscriptions.Contracts;
using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Subscriptions.Queries.GetAllSubscriptions;

public sealed record GetAllSubscriptionsQuery() : IQuery<Result<ImmutableList<SubscriptionResponse>>>;