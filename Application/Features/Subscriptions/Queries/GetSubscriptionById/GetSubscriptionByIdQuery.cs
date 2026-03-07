using Application.Features.Subscriptions.Contracts;
using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Subscriptions.Queries.GetMemberById;

public sealed record GetSubscriptionByIdQuery
(
    long subscriptionId
) : IQuery<Result<SubscriptionResponse>>;
