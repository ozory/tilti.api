using Application.Features.Subscriptions.Contracts;
using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Subscriptions.Queries.GetSubscriptionByAsaasId;

public sealed record GetSubscriptionByAsaasIdQuery(string asaasSubscriptionId) : IQuery<Result<SubscriptionResponse>>;