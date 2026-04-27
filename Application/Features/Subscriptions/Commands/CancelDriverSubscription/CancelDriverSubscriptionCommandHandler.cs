using Application.Shared.Abstractions;
using Domain.Features.Subscriptions.Repository;
using Domain.Shared.Abstractions;
using Domain.Subscriptions.Enums;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Features.Subscriptions.Commands.CancelDriverSubscription;

/// <summary>
/// Handler for CancelDriverSubscriptionCommand
/// </summary>
public class CancelDriverSubscriptionCommandHandler : ICommandHandler<CancelDriverSubscriptionCommand, bool>
{
    private readonly IDriverSubscriptionRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CancelDriverSubscriptionCommandHandler> _logger;
    private readonly string className = nameof(CancelDriverSubscriptionCommandHandler);

    public CancelDriverSubscriptionCommandHandler(
        IDriverSubscriptionRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<CancelDriverSubscriptionCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(CancelDriverSubscriptionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{className}] Canceling driver subscription: {SubscriptionId}", className, request.subscriptionId);

        try
        {
            var subscription = await _repository.GetByIdAsync(request.subscriptionId);

            if (subscription == null)
                return Result.Fail("Subscription not found");

            // Only allow cancellation if subscription is active or pending
            if (subscription.Status != SubscriptionStatus.Active &&
                subscription.Status != SubscriptionStatus.PendingApproval)
            {
                return Result.Fail("Cannot cancel subscription in current status");
            }

            // Cancel subscription
            subscription.SetStatus(SubscriptionStatus.Canceled);

            await _repository.UpdateAsync(subscription);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("[{className}] Driver subscription canceled: {SubscriptionId}. Reason: {Reason}",
                className, request.subscriptionId, request.reason ?? "Not specified");

            return Result.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error canceling subscription: {Error}", className, ex.Message);
            return Result.Fail($"Error canceling subscription: {ex.Message}");
        }
    }
}