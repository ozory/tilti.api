using Application.Shared.Abstractions;
using Domain.Features.Subscriptions.Repository;
using FluentResults;
using Application.Features.Subscriptions.Contracts;

namespace Application.Features.Subscriptions.Queries.GetDriverSubscription;

/// <summary>
/// Handler for GetDriverSubscriptionQuery
/// </summary>
public class GetDriverSubscriptionQueryHandler : IQueryHandler<GetDriverSubscriptionQuery, DriverSubscriptionResponse>
{
    private readonly IDriverSubscriptionRepository _repository;
    private readonly string className = nameof(GetDriverSubscriptionQueryHandler);

    public GetDriverSubscriptionQueryHandler(IDriverSubscriptionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<DriverSubscriptionResponse>> Handle(GetDriverSubscriptionQuery request, CancellationToken cancellationToken)
    {
        var subscription = await _repository.GetSubscriptionByUser(request.userId);

        if (subscription == null)
            return Result.Fail("Driver subscription not found");

        return Result.Ok((DriverSubscriptionResponse)subscription);
    }
}