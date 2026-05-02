using Application.Shared.Abstractions;
using Domain.Features.Subscriptions.Entities;
using Domain.Features.Subscriptions.Repository;
using Domain.Features.Users.Repository;
using FluentResults;
using Microsoft.Extensions.Logging;
using Domain.Features.Plans.Repository;
using FluentValidation;
using Application.Features.Subscriptions.Contracts;

namespace Application.Features.Subscriptions.Commands.CreateDriverSubscription;

/// <summary>
/// Handler for CreateDriverSubscriptionCommand
/// </summary>
public class CreateDriverSubscriptionCommandHandler : ICommandHandler<CreateDriverSubscriptionCommand, DriverSubscriptionResponse>
{
    private readonly IDriverSubscriptionRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly IPlanRepository _planRepository;
    private readonly IDriverPaymentService _driverPaymentService;
    private readonly ILogger<CreateDriverSubscriptionCommandHandler> _logger;
    private readonly IValidator<CreateDriverSubscriptionCommand> _validator;
    private readonly string className = nameof(CreateDriverSubscriptionCommandHandler);

    public CreateDriverSubscriptionCommandHandler(
        ILogger<CreateDriverSubscriptionCommandHandler> logger,
        IDriverSubscriptionRepository repository,
        IUserRepository userRepository,
        IPlanRepository planRepository,
        IDriverPaymentService driverPaymentService,
        IValidator<CreateDriverSubscriptionCommand> validator)
    {
        _repository = repository;
        _logger = logger;
        _userRepository = userRepository;
        _planRepository = planRepository;
        _driverPaymentService = driverPaymentService;
        _validator = validator;
    }

    public async Task<Result<DriverSubscriptionResponse>> Handle(CreateDriverSubscriptionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{className}] Criando assinatura de driver", className);
        try
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

            // Check if user already has an active driver subscription
            var currentSubscription = await _repository.GetSubscriptionByUser(request.userId);
            if (currentSubscription != null && currentSubscription.Status == Domain.Subscriptions.Enums.SubscriptionStatus.Active)
                return Result.Fail("This user already has an active driver subscription!");

            var user = await _userRepository.GetByIdAsync(request.userId);
            if (user == null) return Result.Fail("User not found");

            // Check if user is a driver
            if (!user.IsDriver)
            {
                // Enable user as driver
                user.SetIsDriver(true);
                await _userRepository.UpdateAsync(user);
            }

            var plan = await _planRepository.GetByIdAsync(request.planId);
            if (plan == null) return Result.Fail("Plan not found");
            if (plan.Status != Domain.Plans.Enums.PlanStatus.Active) return Result.Fail("This plan is not active!");

            // Create driver subscription
            var subscription = DriverSubscription.Create(
                null,
                user!,
                plan!,
                DateTime.Now);

            DriverSubscription savedSub = await _repository.SaveAsync(subscription);

            // Create subscription in Asaas
            try
            {
                var (subscriptionId, paymentLink) = await _driverPaymentService.CreateSubscriptionAsync(savedSub, cancellationToken);
                savedSub.SetAsaasSubscriptionId(subscriptionId);
                savedSub.SetAsaasPaymentLink(paymentLink);
                await _repository.UpdateAsync(savedSub);
            }
            catch (Exception ex)
            {
                _logger.LogError("[{className}] Failed to create subscription: {Error}", className, ex.Message);
                // Continue without subscription - can be created later
            }

            _logger.LogInformation("[{className}] Driver subscription created with ID: {Id}", className, savedSub.Id);

            return Result.Ok((DriverSubscriptionResponse)(savedSub));
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error on creating driver subscription: {request} Error: {ex}", className, request, ex);
            throw;
        }
    }
}