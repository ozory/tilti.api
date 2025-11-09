using System.Collections.Immutable;
using Application.Features.Subscriptions.Contracts;
using Application.Shared.Abstractions;
using Domain.Features.Subscriptions.Repository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Features.Subscriptions.Queries.GetAllSubscriptions;

public class GetAllSubscriptionsQueryHandler : IQueryHandler<GetAllSubscriptionsQuery, ImmutableList<SubscriptionResponse>>
{
    private readonly ISubscriptionRepository _repository;
    private readonly ILogger<GetAllSubscriptionsQueryHandler> _logger;
    private readonly string className = nameof(GetAllSubscriptionsQueryHandler);

    public GetAllSubscriptionsQueryHandler(
        ISubscriptionRepository repository,
        ILogger<GetAllSubscriptionsQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<ImmutableList<SubscriptionResponse>>> Handle(
        GetAllSubscriptionsQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{className}] Getting all subscriptions", className);
        try
        {
            var subscriptions = await _repository.GetAllAsync();
            return Result.Ok(subscriptions.Select(x => (SubscriptionResponse)x).ToImmutableList());
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error getting all subscriptions: {Error}", className, ex);
            throw;
        }
    }
}