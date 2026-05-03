# Quickstart: Passenger Travel Payment Flow

**Date**: May 3, 2026  
**Feature**: [spec.md](../spec.md)  
**Plan**: [plan.md](../plan.md)

## Overview

This guide shows how to implement the passenger travel payment flow, from pricing to order creation after payment confirmation.

## Prerequisites

- .NET 10 SDK
- PostgreSQL database configured
- Asaas API credentials in `appsettings.json`:
  ```json
  {
    "Configurations": {
      "PaymentUrl": "https://sandbox.asaas.com/api/v3",
      "PaymentToken": "your-asaas-api-key"
    }
  }
  ```

## Implementation Steps

### Step 1: Add PassengerRide to PaymentType Enum

**File**: `Domain/Features/Payments/Enums/PaymentType.cs`

```csharp
public enum PaymentType : ushort
{
    CreditCard = 1,
    PIX = 2,
    PassengerRide = 3  // NEW
}
```

---

### Step 2: Create Passenger Payment Service

**File**: `Infrastructure/External/Features/Payments/Services/PassengerPaymentService.cs`

```csharp
using System.Text.Json;
using Domain.Features.Payments.Enums;
using Infrastructure.External.Features.Payments.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Infrastructure.External.Features.Payments.Services;

public class PassengerPaymentService : IPassengerPaymentService
{
    private readonly ILogger<PassengerPaymentService> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _baseUrl;
    private readonly string _apiToken;

    public PassengerPaymentService(
        ILogger<PassengerPaymentService> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _baseUrl = configuration.GetSection("Configurations:PaymentUrl").Value!;
        _apiToken = configuration.GetSection("Configurations:PaymentToken").Value!;
    }

    public async Task<string> CreateOrGetWalletAsync(long userId, CancellationToken cancellationToken = default)
    {
        // Reuse logic from DriverPaymentService.CreateDriverWalletAsync()
        // Check if user has wallet → return it
        // Otherwise create new wallet via POST /v3/accounts
    }

    public async Task<(string paymentId, string pixLink, string qrCode)> CreatePaymentAsync(
        long orderId,
        decimal amount,
        string customerWalletId,
        DateTime dueDate,
        CancellationToken cancellationToken = default)
    {
        var client = new RestClient(new RestClientOptions(_baseUrl));
        var request = new RestRequest("v3/paymentLinks", Method.Post);

        var paymentRequest = new PaymentLinkRequest(
            name: $"Corrida {orderId}",
            billingType: "PIX",
            chargeType: "DETACHED",  // Single payment
            value: amount,
            description: $"Pagamento de corrida - Order {orderId}",
            dueDate: dueDate,
            installmentCount: 1,
            active: true,
            externalReference: orderId.ToString()
        );

        // Serialize and send request (follow DriverPaymentService pattern)
        // Return paymentId, pixLink, qrCode
    }

    public async Task<bool> CheckPaymentStatusAsync(string asaasPaymentId, CancellationToken cancellationToken = default)
    {
        // GET /v3/payments/{asaasPaymentId}
        // Check if status == "CONFIRMED"
    }
}
```

---

### Step 3: Create Payment Contracts

**File**: `Infrastructure/External/Features/Payments/Contracts/PassengerPaymentContracts.cs`

```csharp
public record PaymentLinkRequest(
    string name,
    string billingType,
    string chargeType,
    decimal value,
    string description,
    DateTime dueDate,
    int installmentCount,
    bool active,
    string externalReference
);

public record PaymentLinkResponse(
    string id,
    string url,
    string qrCode
);
```

---

### Step 4: Create CreatePassengerPaymentCommand

**File**: `Application/Features/Payments/Commands/CreatePassengerPayment/CreatePassengerPaymentCommand.cs`

```csharp
using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Payments.Commands.CreatePassengerPayment;

public record CreatePassengerPaymentCommand(
    long UserId,
    decimal Amount,
    string ExternalReference,
    DateTime DueDate
) : ICommand<CreatePassengerPaymentCommand, PaymentResponse>;
```

**File**: `Application/Features/Payments/Commands/CreatePassengerPayment/CreatePassengerPaymentCommandHandler.cs`

```csharp
using Application.Features.Orders.Contracts;
using Application.Features.Payments.Contracts;
using Application.Shared.Abstractions;
using Domain.Features.Users.Repository;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Payments.Commands.CreatePassengerPayment;

public class CreatePassengerPaymentCommandHandler : ICommandHandler<CreatePassengerPaymentCommand, PaymentResponse>
{
    private readonly IPassengerPaymentService _paymentService;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CreatePassengerPaymentCommandHandler> _logger;
    private readonly IValidator<CreatePassengerPaymentCommand> _validator;
    private readonly string _className = nameof(CreatePassengerPaymentCommandHandler);

    public CreatePassengerPaymentCommandHandler(
        ILogger<CreatePassengerPaymentCommandHandler> logger,
        IPassengerPaymentService paymentService,
        IUserRepository userRepository,
        IValidator<CreatePassengerPaymentCommand> validator)
    {
        _logger = logger;
        _paymentService = paymentService;
        _userRepository = userRepository;
        _validator = validator;
    }

    public async Task<Result<PaymentResponse>> Handle(
        CreatePassengerPaymentCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{className}] Creating passenger payment for User {UserId}", 
            _className, request.UserId);

        try
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid) 
                return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

            // Get user and check/create wallet
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null) return Result.Fail("User not found");

            // Create or get wallet (reuse DriverPaymentService pattern)
            var walletId = await _paymentService.CreateOrGetWalletAsync(request.UserId, cancellationToken);

            // Create payment in Asaas
            var (paymentId, pixLink, qrCode) = await _paymentService.CreatePaymentAsync(
                orderId: 0, // Will be set after order creation
                amount: request.Amount,
                customerWalletId: walletId,
                dueDate: request.DueDate,
                cancellationToken);

            return Result.Ok(new PaymentResponse
            {
                PaymentId = paymentId,
                PixLink = pixLink,
                QrCode = qrCode,
                Amount = request.Amount,
                Status = "PENDING"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error creating payment: {Exception}", _className, ex);
            return Result.Fail(ex.Message);
        }
    }
}
```

---

### Step 5: Modify CreateOrderCommand to Require Payment

**File**: `Application/Features/Orders/Commands/Create/CreateOrderCommand.cs`

Add parameter:
```csharp
public record CreateOrderCommand(
    // ... existing params ...
    string AsaasPaymentId  // NEW - Required for payment validation
);
```

**File**: `Application/Features/Orders/Commands/Create/CreateOrderCommandHandler.cs`

Modify the Handle method to:
1. Validate payment exists and is approved (via Asaas API or Payment repository)
2. Set Order status to `ReadyToAccept` (not `PendingPayment`)
3. Link Payment to Order

---

### Step 6: Create Webhook Endpoint for Payment Confirmation

**File**: `Api/Endpoints/WebhookEndpoint.cs`

Add new endpoint for passenger ride payments:
```csharp
webhooks.MapPost("/asaas/passenger-ride", HandlePassengerRideWebhook)
    .WithName("Webhook.AsaasPassengerRide");
```

**Handler logic**:
1. Extract `externalReference` (Order ID)
2. Verify payment status is "CONFIRMED"
3. Call `CreateOrderCommand` with payment ID
4. Update Payment entity status to `Approved`

---

## Testing the Flow

### 1. Price the Order
```bash
POST /api/orders/precify
{
  "userId": 123,
  "addresses": [{"type": "origin", ...}, {"type": "destination", ...}]
}
```
Response: Returns priced order with amount

### 2. Create Payment
```bash
POST /api/payments/passenger/create
{
  "userId": 123,
  "amount": 25.50,
  "externalReference": "order-456",
  "dueDate": "2026-05-03T15:30:00Z"
}
```
Response: Returns PIX link and QR code

### 3. Simulate Payment (Sandbox)
- Use Asaas sandbox to simulate PIX payment
- Or manually call: `POST /api/webhooks/asaas/passenger-ride` with webhook payload

### 4. Verify Order Created
```bash
GET /api/orders/{orderId}
```
Response: Order with status `ReadyToAccept`

---

## Common Issues

### Wallet Creation Fails
- Check if user has CPF (`user.Document.Value`)
- Verify Asaas API credentials
- Check sandbox vs production environment

### Payment Link Not Generated
- Verify `PaymentUrl` and `PaymentToken` in config
- Check Asaas API response in logs
- Ensure amount > 0

### Order Not Created After Payment
- Check webhook is being called
- Verify `externalReference` is being parsed correctly
- Check `CreateOrderCommand` validation

---

## Next Steps

1. Run `/speckit.tasks` to generate detailed task list
2. Implement tasks in order: Domain → Infrastructure → Application → API
3. Test complete flow end-to-end
4. Run `/speckit.analyze` to verify implementation
