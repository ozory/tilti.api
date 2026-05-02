using Application.Features.Subscriptions.Contracts;
using Application.Shared.Abstractions;
using Domain.Features.Subscriptions.Repository;
using Domain.Subscriptions.Enums;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Features.Subscriptions.Commands.InactivateDriverSubscription;

public class InactivateDriverSubscriptionCommandHandler : ICommandHandler<InactivateDriverSubscriptionCommand, DriverSubscriptionResponse>
{
    private readonly IDriverSubscriptionRepository _repository;
    private readonly ILogger<InactivateDriverSubscriptionCommandHandler> _logger;
    private readonly string _className = nameof(InactivateDriverSubscriptionCommandHandler);

    public InactivateDriverSubscriptionCommandHandler(
        ILogger<InactivateDriverSubscriptionCommandHandler> logger,
        IDriverSubscriptionRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<Result<DriverSubscriptionResponse>> Handle(InactivateDriverSubscriptionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{className}] Inactivating subscription: {Id}", _className, request.AsaasSubscriptionId);

        try
        {
            var subscription = await _repository.GetByAsaasSubscriptionId(request.AsaasSubscriptionId);

            if (subscription == null)
                return Result.Fail("Subscription not found");

            subscription.SetStatus(SubscriptionStatus.Inactive);
            await _repository.UpdateAsync(subscription);

            _logger.LogInformation("[{className}] Subscription inactivated successfully: {Id}", _className, subscription.Id);

            return Result.Ok((DriverSubscriptionResponse)subscription);
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error inactivating subscription: {error}", _className, ex.Message);
            return Result.Fail(ex.Message);
        }
    }
}
