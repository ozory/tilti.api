# API Contracts: Unify Subscriptions

**Date**: 2026-05-03  
**Feature**: 002-unify-subscriptions  
**Purpose**: Define API contracts for the unified Subscription entity.

## Contract: SubscriptionResponse (Updated)

**File**: `Application/Features/Subscriptions/Contracts/SubscriptionResponse.cs`

```csharp
namespace Application.Features.Subscriptions.Contracts;

public class SubscriptionResponse
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long PlanId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string SubscriptionType { get; set; } = string.Empty; // NEW
    public DateTime DueDate { get; set; }
    public string? PaymentToken { get; set; }
    public string? AsaasPaymentId { get; set; } // FROM DriverSubscription
    public string? AsaasPaymentLink { get; set; } // FROM DriverSubscription
    public string? AsaasSubscriptionId { get; set; } // FROM DriverSubscription
    public DateTime? PaidAt { get; set; } // FROM DriverSubscription
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties (for display)
    public string? UserName { get; set; }
    public string? PlanName { get; set; }
    
    // Explicit conversion from Subscription entity
    public static implicit operator SubscriptionResponse(Domain.Features.Subscriptions.Entities.Subscription subscription)
    {
        return new SubscriptionResponse
        {
            Id = subscription.Id,
            UserId = subscription.UserId,
            PlanId = subscription.PlanId,
            Status = subscription.Status.ToString(),
            SubscriptionType = subscription.SubscriptionType.ToString(), // NEW
            DueDate = subscription.DueDate,
            PaymentToken = subscription.PaymentToken,
            AsaasPaymentId = subscription.AsaasPaymentId, // FROM DriverSubscription
            AsaasPaymentLink = subscription.AsaasPaymentLink, // FROM DriverSubscription
            AsaasSubscriptionId = subscription.AsaasSubscriptionId, // FROM DriverSubscription
            PaidAt = subscription.PaidAt, // FROM DriverSubscription
            CreatedAt = subscription.CreatedAt,
            UpdatedAt = subscription.UpdatedAt,
            UserName = subscription.User?.Name,
            PlanName = subscription.Plan?.Name
        };
    }
}
```

## Contract: CreateSubscriptionCommand (Updated)

**File**: `Application/Features/Subscriptions/Commands/CreateSubscription/CreateSubscriptionCommand.cs`

```csharp
using Application.Shared.Abstractions;
using Application.Features.Subscriptions.Contracts;
using Domain.Features.Subscriptions.Entities;
using FluentValidation;

namespace Application.Features.Subscriptions.Commands.CreateSubscription;

public class CreateSubscriptionCommand : IRequest<SubscriptionResponse>, ICommand<SubscriptionResponse>
{
    public long UserId { get; set; }
    public long PlanId { get; set; }
    public string SubscriptionType { get; set; } = "Driver"; // NEW: Default to Driver
    
    // For backward compatibility with existing DriverSubscription calls
    public string? AsaasPaymentId { get; set; }
    public string? AsaasPaymentLink { get; set; }
    public string? AsaasSubscriptionId { get; set; }
}

public class CreateSubscriptionCommandValidator : AbstractValidator<CreateSubscriptionCommand>
{
    public CreateSubscriptionCommandValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.PlanId).GreaterThan(0);
        RuleFor(x => x.SubscriptionType)
            .Must(type => type == "Driver" || type == "Passenger")
            .WithMessage("SubscriptionType must be 'Driver' or 'Passenger'");
    }
}

public class CreateSubscriptionCommandHandler : ICommandHandler<CreateSubscriptionCommand, SubscriptionResponse>
{
    private readonly ISubscriptionRepository _repository;
    private readonly ILogger<CreateSubscriptionCommandHandler> _logger;
    private readonly IValidator<CreateSubscriptionCommand> _validator;
    private readonly string _className = nameof(CreateSubscriptionCommandHandler);

    public CreateSubscriptionCommandHandler(
        ILogger<CreateSubscriptionCommandHandler> logger,
        IValidator<CreateSubscriptionCommand> validator,
        ISubscriptionRepository repository)
    {
        _logger = logger;
        _validator = validator;
        _repository = repository;
    }

    public async Task<Result<SubscriptionResponse>> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{className}] Creating Subscription for User: {UserId}, Type: {SubscriptionType}", 
            _className, request.UserId, request.SubscriptionType);

        try
        {
            // Validate command
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

            // Check for existing active subscription of same type
            var existingSubscription = await _repository.GetActiveSubscriptionByUserAndType(
                request.UserId, 
                Enum.Parse<Domain.Subscriptions.Enums.SubscriptionType>(request.SubscriptionType));
            
            if (existingSubscription != null)
                return Result.Fail($"User already has an active {request.SubscriptionType} subscription");

            // Get user and plan (simplified - actual implementation may need repository calls)
            // ... (implementation depends on existing patterns)

            // Create subscription
            var subscription = Subscription.Create(
                null,
                user,
                plan,
                DateTime.Now);
            
            subscription.SetSubscriptionType(Enum.Parse<Domain.Subscriptions.Enums.SubscriptionType>(request.SubscriptionType));
            
            // Set Asaas properties if provided
            if (!string.IsNullOrEmpty(request.AsaasPaymentId))
                subscription.SetAsaasPaymentId(request.AsaasPaymentId);
            // ... (similar for other properties)

            // Save
            var savedSubscription = await _repository.SaveAsync(subscription, cancellationToken);

            _logger.LogInformation("[{className}] Subscription created successfully: {SubscriptionId}", 
                _className, savedSubscription.Id);
                
            return Result.Ok((SubscriptionResponse)savedSubscription);
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error creating Subscription: {Exception}", _className, ex);
            return Result.Fail(ex.Message);
        }
    }
}
```

## Contract: GetSubscriptionQuery (Updated)

**File**: `Application/Features/Subscriptions/Queries/GetSubscription/GetSubscriptionQuery.cs`

```csharp
using Application.Shared.Abstractions;
using Application.Features.Subscriptions.Contracts;

namespace Application.Features.Subscriptions.Queries.GetSubscription;

public class GetSubscriptionQuery : IRequest<SubscriptionResponse>, IQuery<SubscriptionResponse>
{
    public long SubscriptionId { get; set; }
}

public class GetSubscriptionQueryHandler : IQueryHandler<GetSubscriptionQuery, SubscriptionResponse>
{
    private readonly ISubscriptionRepository _repository;
    private readonly ILogger<GetSubscriptionQueryHandler> _logger;
    private readonly string _className = nameof(GetSubscriptionQueryHandler);

    public GetSubscriptionQueryHandler(
        ILogger<GetSubscriptionQueryHandler> logger,
        ISubscriptionRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<Result<SubscriptionResponse>> Handle(GetSubscriptionQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var subscription = await _repository.GetByIdAsync(request.SubscriptionId, cancellationToken);
            if (subscription == null)
                return Result.Fail("Subscription not found");

            return Result.Ok((SubscriptionResponse)subscription);
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error getting Subscription: {Exception}", _className, ex);
            return Result.Fail(ex.Message);
        }
    }
}
```

## API Endpoints (Updated)

### Create Subscription
**POST** `/subscriptions`

**Request Body**:
```json
{
    "userId": 123,
    "planId": 456,
    "subscriptionType": "Driver" // or "Passenger"
}
```

**Response** (201 Created):
```json
{
    "id": 789,
    "userId": 123,
    "planId": 456,
    "status": "PendingApproval",
    "subscriptionType": "Driver",
    "dueDate": "2026-06-03T00:00:00Z",
    "asaasPaymentId": null,
    "asaasPaymentLink": null,
    "asaasSubscriptionId": null,
    "paidAt": null,
    "createdAt": "2026-05-03T10:00:00Z",
    "updatedAt": "2026-05-03T10:00:00Z"
}
```

### Get Subscription
**GET** `/subscriptions/{id}`

**Response** (200 OK): Same as Create response above.

### Activate Subscription
**POST** `/subscriptions/activate`

**Request Body**:
```json
{
    "subscriptionId": 789,
    "asaasPaymentId": "pay_123456",
    "asaasPaymentLink": "https://..."
}
```

### Cancel Subscription
**POST** `/subscriptions/cancel`

**Request Body**:
```json
{
    "subscriptionId": 789
}
```

## Backward Compatibility

For existing code that used `CreateDriverSubscriptionCommand`:
- Old endpoint: `POST /subscriptions/driver`
- New endpoint: `POST /subscriptions` (with `subscriptionType: "Driver"`)
- The `CreateSubscriptionCommand` accepts the same properties as the old `CreateDriverSubscriptionCommand`
- Webhook handlers updated to use unified `Subscription` entity
