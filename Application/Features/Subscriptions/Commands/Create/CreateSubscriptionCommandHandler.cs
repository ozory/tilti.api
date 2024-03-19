using Application.Shared.Abstractions;
using Domain.Features.Subscriptions.Entities;
using Domain.Features.Subscriptions.Repository;
using Domain.Features.Users.Repository;
using FluentResults;
using Microsoft.Extensions.Logging;
using Domain.Features.Plans.Repository;
using FluentValidation;
using Application.Features.Subscriptions.Contracts;

namespace Application.Features.Subscriptions.Commands.Create;

public class CreateSubscriptionCommandHandler : ICommandHandler<CreateSubscriptionCommand, SubscriptionResponse>
{
    private readonly ISubscriptionRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly IPlanRepository _planRepository;
    private readonly ILogger<CreateSubscriptionCommandHandler> _logger;
    private readonly IValidator<CreateSubscriptionCommand> _validator;

    public CreateSubscriptionCommandHandler(
        ILogger<CreateSubscriptionCommandHandler> logger,
        ISubscriptionRepository repository,
        IUserRepository userRepository,
        IPlanRepository planRepository,
        IValidator<CreateSubscriptionCommand> validator)
    {
        _repository = repository;
        _logger = logger;
        _userRepository = userRepository;
        _planRepository = planRepository;
        _validator = validator;
    }

    public async Task<Result<SubscriptionResponse>> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Criando assinatura");

        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

        var currentSubscription = await _repository.GetSubscriptionByUser(request.userId);
        if (currentSubscription != null) return Result.Fail("This user allready have a Subscription!");

        var user = await _userRepository.GetByIdAsync(request.userId);
        if (user == null) return Result.Fail("User not Found");
        if (user.Status != Domain.Enums.UserStatus.Active) return Result.Fail("This user can't get a Subscription!");
        if (!user.DriveEnable) return Result.Fail("Only a driver can use Subscription!");

        var plan = await _planRepository.GetByIdAsync(request.planId);
        if (plan == null) return Result.Fail("Plan not Found");
        if (plan.Status != Domain.Plans.Enums.PlanStatus.Active) return Result.Fail("Its not possible to use this Plan!");

        var subscription = Subscription.Create(
            null,
            user!,
            plan!,
            DateTime.Now);

        Subscription savedSub = await _repository.SaveAsync(subscription);

        _logger.LogInformation("Assinatura criada com sucesso: {Id}", savedSub.Id);

        return Result.Ok((SubscriptionResponse)(savedSub));
    }

}
