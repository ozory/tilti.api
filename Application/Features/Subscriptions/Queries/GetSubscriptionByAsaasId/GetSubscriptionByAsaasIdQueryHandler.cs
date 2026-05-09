using Application.Features.Subscriptions.Contracts;
using Application.Shared.Abstractions;
using Domain.Features.Subscriptions.Repository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Features.Subscriptions.Queries.GetSubscriptionByAsaasId;

public class GetSubscriptionByAsaasIdQueryHandler : IQueryHandler<GetSubscriptionByAsaasIdQuery, SubscriptionResponse>
{
    private readonly ISubscriptionRepository _repository;
    private readonly ILogger<GetSubscriptionByAsaasIdQueryHandler> _logger;
    private readonly string _className = nameof(GetSubscriptionByAsaasIdQueryHandler);

    public GetSubscriptionByAsaasIdQueryHandler(
        ISubscriptionRepository repository,
        ILogger<GetSubscriptionByAsaasIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<SubscriptionResponse>> Handle(
        GetSubscriptionByAsaasIdQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{ClassName}] Getting subscription by Asaas ID: {AsaasSubscriptionId}",
            _className, request.asaasSubscriptionId);

        try
        {
            var subscription = await _repository.GetByAsaasSubscriptionId(request.asaasSubscriptionId);
            if (subscription == null)
                return Result.Fail("Subscription not found");

            return Result.Ok((SubscriptionResponse)subscription);
        }
        catch (Exception ex)
        {
            _logger.LogError("[{ClassName}] Error getting subscription by Asaas ID: {Error}",
                _className, ex.Message);
            return Result.Fail($"Error getting subscription: {ex.Message}");
        }
    }
}