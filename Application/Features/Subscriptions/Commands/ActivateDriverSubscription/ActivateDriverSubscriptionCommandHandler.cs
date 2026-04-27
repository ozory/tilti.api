using Application.Shared.Abstractions;
using Domain.Features.Subscriptions.Entities;
using Domain.Features.Subscriptions.Repository;
using Domain.Shared.Abstractions;
using Domain.Subscriptions.Enums;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Features.Subscriptions.Commands.ActivateDriverSubscription;

/// <summary>
/// Handler for ActivateDriverSubscriptionCommand
/// </summary>
public class ActivateDriverSubscriptionCommandHandler : ICommandHandler<ActivateDriverSubscriptionCommand, bool>
{
    private readonly IDriverSubscriptionRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ActivateDriverSubscriptionCommandHandler> _logger;
    private readonly string className = nameof(ActivateDriverSubscriptionCommandHandler);

    public ActivateDriverSubscriptionCommandHandler(
        IDriverSubscriptionRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<ActivateDriverSubscriptionCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(ActivateDriverSubscriptionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{className}] Activating driver subscription: {SubscriptionId}", className, request.subscriptionId);

        try
        {
            DriverSubscription? subscription;

            // If subscriptionId is provided, use it directly
            if (request.subscriptionId > 0)
            {
                subscription = await _repository.GetByIdAsync(request.subscriptionId);
            }
            // Otherwise, find by Asaas payment ID
            else if (!string.IsNullOrEmpty(request.asaasPaymentId))
            {
                subscription = await _repository.GetByAsaasPaymentId(request.asaasPaymentId);
            }
            else
            {
                return Result.Fail("Either subscriptionId or asaasPaymentId must be provided");
            }

            if (subscription == null)
                return Result.Fail("Subscription not found");

            // Verify payment with Asaas (if subscriptionId was provided)
            if (request.subscriptionId > 0 && subscription.AsaasPaymentId != request.asaasPaymentId)
                return Result.Fail("Payment ID mismatch");

            // Activate subscription
            subscription.SetStatus(SubscriptionStatus.Active);
            subscription.MarkAsPaid();

            await _repository.UpdateAsync(subscription);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("[{className}] Driver subscription activated: {SubscriptionId}", className, request.subscriptionId);

            return Result.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error activating subscription: {Error}", className, ex.Message);
            return Result.Fail($"Error activating subscription: {ex.Message}");
        }
    }
}