using Application.Shared.Abstractions;
using Domain.Features.Subscriptions.Repository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Features.Subscriptions.Commands.DeleteDriverSubscription;

public class DeleteDriverSubscriptionCommandHandler : ICommandHandler<DeleteDriverSubscriptionCommand>
{
    private readonly IDriverSubscriptionRepository _repository;
    private readonly ILogger<DeleteDriverSubscriptionCommandHandler> _logger;
    private readonly string _className = nameof(DeleteDriverSubscriptionCommandHandler);

    public DeleteDriverSubscriptionCommandHandler(
        ILogger<DeleteDriverSubscriptionCommandHandler> logger,
        IDriverSubscriptionRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<Result> Handle(DeleteDriverSubscriptionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{className}] Deleting subscription: {AsaasSubscriptionId}", _className, request.AsaasSubscriptionId);

        try
        {
            var subscription = await _repository.GetByAsaasSubscriptionId(request.AsaasSubscriptionId);

            if (subscription == null)
                return Result.Fail("Subscription not found");

            await _repository.RemoveAsync(subscription);

            _logger.LogInformation("[{className}] Subscription deleted successfully: {Id}", _className, subscription.Id);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error deleting subscription: {error}", _className, ex.Message);
            return Result.Fail(ex.Message);
        }
    }
}