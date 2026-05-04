using Application.Shared.Abstractions;
using Domain.Features.Subscriptions.Entities;
using Domain.Features.Subscriptions.Repository;
using Domain.Features.Users.Repository;
using FluentResults;
using Microsoft.Extensions.Logging;
using Domain.Features.Plans.Repository;
using FluentValidation;
using Application.Features.Subscriptions.Contracts;
using Domain.Subscriptions.Enums;

namespace Application.Features.Subscriptions.Commands.CreateSubscription;

/// <summary>
/// Handler for CreateSubscriptionCommand
/// </summary>
public class CreateSubscriptionCommandHandler : ICommandHandler<CreateSubscriptionCommand, SubscriptionResponse>
{
    private readonly ISubscriptionRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly IPlanRepository _planRepository;
    private readonly ISubscriptionPaymentService _driverPaymentService;
    private readonly ILogger<CreateSubscriptionCommandHandler> _logger;
    private readonly IValidator<CreateSubscriptionCommand> _validator;
    private readonly string className = nameof(CreateSubscriptionCommandHandler);

    public CreateSubscriptionCommandHandler(
        ILogger<CreateSubscriptionCommandHandler> logger,
        ISubscriptionRepository repository,
        IUserRepository userRepository,
        IPlanRepository planRepository,
        ISubscriptionPaymentService driverPaymentService,
        IValidator<CreateSubscriptionCommand> validator)
    {
        _repository = repository;
        _logger = logger;
        _userRepository = userRepository;
        _planRepository = planRepository;
        _driverPaymentService = driverPaymentService;
        _validator = validator;
    }

    public async Task<Result<SubscriptionResponse>> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{className}] Criando assinatura: {SubscriptionType}", className, request.subscriptionType);
        try
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

            // Check if user already has an active subscription of the same type
            var currentSubscription = await _repository.GetActiveSubscriptionByUserAndType(request.userId, request.subscriptionType);
            if (currentSubscription != null)
                return Result.Fail($"This user already has an active {request.subscriptionType} subscription!");

            var user = await _userRepository.GetByIdAsync(request.userId);
            if (user == null) return Result.Fail("User not found");

            // If Driver subscription, ensure user is a driver
            if (request.subscriptionType == SubscriptionType.Driver && !user.IsDriver)
            {
                user.SetIsDriver(true);
                await _userRepository.UpdateAsync(user);
            }

            var plan = await _planRepository.GetByIdAsync(request.planId);
            if (plan == null) return Result.Fail("Plan not found");
            if (plan.Status != Domain.Plans.Enums.PlanStatus.Active) return Result.Fail("This plan is not active!");

            // Create subscription with SubscriptionType
            var subscription = Subscription.Create(
                null,
                user!,
                plan!,
                DateTime.Now,
                request.subscriptionType // NEW: Pass SubscriptionType
            );

            var savedSub = await _repository.SaveAsync(subscription);

            // Create subscription in Asaas (only for Driver subscriptions)
            if (request.subscriptionType == SubscriptionType.Driver)
            {
                try
                {
                    var (subscriptionId, paymentLink) = await _driverPaymentService.CreateSubscriptionAsync(savedSub, request.subscriptionType, cancellationToken);
                    savedSub.SetAsaasSubscriptionId(subscriptionId);
                    savedSub.SetAsaasPaymentLink(paymentLink);
                    await _repository.UpdateAsync(savedSub);
                }
                catch (Exception ex)
                {
                    _logger.LogError("[{className}] Failed to create Asaas subscription: {Error}", className, ex.Message);
                }
            }

            _logger.LogInformation("[{className}] Subscription created with ID: {Id}, Type: {Type}", className, savedSub.Id, request.subscriptionType);

            return Result.Ok((SubscriptionResponse)(savedSub));
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error on creating subscription: {request} Error: {ex}", className, request, ex);
            throw;
        }
    }
}