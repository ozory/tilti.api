using Application.Shared.Abstractions;
using Domain.Features.Subscriptions.Repository;
using Domain.Shared.Abstractions;
using Domain.Subscriptions.Enums;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Features.Subscriptions.Commands.CancelSubscription;

/// <summary>
/// Handler for CancelSubscriptionCommand
/// </summary>
public class CancelSubscriptionCommandHandler : ICommandHandler<CancelSubscriptionCommand, bool>
{
    private readonly ISubscriptionRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CancelSubscriptionCommandHandler> _logger;
    private readonly string className = nameof(CancelSubscriptionCommandHandler);

    public CancelSubscriptionCommandHandler(
        ISubscriptionRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<CancelSubscriptionCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{className}] Canceling subscription: {SubscriptionId}", className, request.subscriptionId);

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

            _logger.LogInformation("[{className}] Subscription canceled: {SubscriptionId}. Reason: {Reason}",
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