using Application.Shared.Abstractions;
using Domain.Features.Subscriptions.Repository;
using FluentResults;
using Application.Features.Subscriptions.Contracts;

namespace Application.Features.Subscriptions.Queries.GetSubscription;

/// <summary>
/// Handler for GetSubscriptionQuery
/// </summary>
public class GetSubscriptionQueryHandler : IQueryHandler<GetSubscriptionQuery, SubscriptionResponse>
{
    private readonly ISubscriptionRepository _repository;

    public GetSubscriptionQueryHandler(ISubscriptionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<SubscriptionResponse>> Handle(GetSubscriptionQuery request, CancellationToken cancellationToken)
    {
        var subscription = await _repository.GetSubscriptionByUser(request.userId);

        if (subscription == null)
            return Result.Fail("Subscription not found");

        return Result.Ok((SubscriptionResponse)subscription);
    }
}