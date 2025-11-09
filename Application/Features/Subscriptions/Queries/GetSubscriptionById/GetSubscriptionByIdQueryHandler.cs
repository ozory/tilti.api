using Application.Features.Subscriptions.Contracts;
using Application.Features.Subscriptions.Queries.GetMemberById;
using Application.Shared.Abstractions;
using Domain.Features.Subscriptions.Repository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Features.Subscriptions.Queries.GetSubscriptionById
{
    public class GetSubscriptionByIdQueryHandler
        : IQueryHandler<GetSubscriptionByIdQuery, SubscriptionResponse>
    {
        private readonly ISubscriptionRepository _repository;
        private readonly ILogger<GetSubscriptionByIdQueryHandler> _logger;
        private readonly string className = nameof(GetSubscriptionByIdQueryHandler);

        public GetSubscriptionByIdQueryHandler(
            ISubscriptionRepository repository,
            ILogger<GetSubscriptionByIdQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result<SubscriptionResponse>> Handle(
            GetSubscriptionByIdQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("[{className}] Getting subscription {SubscriptionId}", className, request.subscriptionId);
            try
            {
                var subscription = await _repository.GetByIdAsync(request.subscriptionId);
                if (subscription == null) return Result.Fail("Subscription not found");

                return Result.Ok((SubscriptionResponse)subscription);
            }
            catch (Exception ex)
            {
                _logger.LogError("[{className}] Error getting subscription {SubscriptionId}: {Error}",
                    className, request.subscriptionId, ex);
                throw;
            }
        }
    }
}