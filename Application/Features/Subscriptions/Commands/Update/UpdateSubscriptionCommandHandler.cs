using Application.Features.Subscriptions.Contracts;
using Application.Shared.Abstractions;
using Domain.Features.Plans.Repository;
using Domain.Features.Subscriptions.Repository;
using Domain.Features.Subscriptions.Entities;
using Domain.Features.Users.Repository;
using Domain.Subscriptions.Enums;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Subscriptions.Commands.Update;

public class UpdateSubscriptionCommandHandler : ICommandHandler<UpdateSubscriptionCommand, SubscriptionResponse>
{
    private readonly ISubscriptionRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly IPlanRepository _planRepository;
    private readonly ILogger<UpdateSubscriptionCommandHandler> _logger;
    private readonly IValidator<UpdateSubscriptionCommand> _validator;


    public UpdateSubscriptionCommandHandler(
        ILogger<UpdateSubscriptionCommandHandler> logger,
        ISubscriptionRepository repository,
        IUserRepository userRepository,
        IPlanRepository planRepository,
        IValidator<UpdateSubscriptionCommand> validator)
    {
        _repository = repository;
        _logger = logger;
        _userRepository = userRepository;
        _planRepository = planRepository;
        _validator = validator;
    }

    public async Task<Result<SubscriptionResponse>> Handle(
        UpdateSubscriptionCommand request,
        CancellationToken cancellationToken)
    {

        _logger.LogInformation("Atualizando assinatura");

        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

        var subscription = await _repository.GetByIdAsync(request.SubscriptionId);
        if (subscription == null) return Result.Fail("Subscription not found");

        if (subscription.Status == SubscriptionStatus.Inactive
            && request.Status == (ushort)SubscriptionStatus.Inactive)
        {
            return Result.Fail("Subscription allready cancelled");
        }

        if (subscription.Status == SubscriptionStatus.Inactive
            && request.Status == (ushort)SubscriptionStatus.Active)
        {
            return Result.Fail("Inactive Subscription must enter in PendingApproval status");
        }

        subscription.SetStatus((SubscriptionStatus)request.Status);
        subscription.SetUpdatedAt(DateTime.Now);
        subscription.SetDueDate(request.DueDate);

        Subscription savedSub = await _repository.UpdateAsync(subscription);

        _logger.LogInformation("Assinatura criada com sucesso: {Id}", savedSub.Id);

        return Result.Ok((SubscriptionResponse)(savedSub));
    }
}
