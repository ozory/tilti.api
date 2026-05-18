# Quickstart: Refund Penalty Consumer

## Overview

This feature implements a RabbitMQ consumer that receives `OrderCanceledPaymentRefund` events and calculates the customer refund penalty based on driver distance. It does not execute refunds yet.

## Setup

1. Configure RabbitMQ settings in `appsettings.json` or `appsettings.Development.json`:

```json
"Infrastructure": {
  "OrderCanceledPaymentRefundMessages": {
    "exchange": "tilt.order.canceled.payment.refund.exchange",
    "queue": "tilt.order.canceled.payment.refund.queue",
    "routingKey": "tilt.order.canceled.payment.refund",
    "exchangeType": "topic",
    "consumerIntances": "1",
    "delayInterval": "5000"
  }
}
```

2. The consumer is automatically registered in `Application.Configurations.DependencyInjection.AddApplication`:

```csharp
services.AddHostedService<OrderCanceledPaymentRefundConsumer>();
services.AddScoped<RefundPenaltyCalculationService>();
```

3. Ensure `IMessageRepository` is already registered by the existing infrastructure DI setup.

## Implementation Details

### Consumer: OrderCanceledPaymentRefundConsumer

Located at: `Application/Features/Orders/Consumers/OrderCanceledPaymentRefundConsumer.cs`

Responsibilities:
- Receives `OrderCanceledPaymentRefundDomainEvent` from RabbitMQ
- Validates message payload (OrderId, Amount)
- Validates order exists and is in Canceled status
- Checks if driver accepted the order (no penalty if not)
- Invokes `RefundPenaltyCalculationService` for penalty calculation
- Logs all calculation details for audit

### Service: RefundPenaltyCalculationService

Located at: `Application/Features/Orders/Services/RefundPenaltyCalculationService.cs`

Responsibilities:
- Calculates penalty percentage based on distance ratio
- Enforces maximum 80% penalty (minimum 20% refund)
- Returns `PenaltyCalculationResult` value object

### Value Object: PenaltyCalculationResult

Located at: `Domain/Features/Orders/ValueObjects/PenaltyCalculationResult.cs`

Properties:
- `OriginalAmount`: Original order amount
- `PenaltyPercentage`: Calculated penalty (0-80%)
- `PenaltyAmount`: Dollar amount of penalty
- `RefundAmount`: Dollar amount to refund (minimum 20%)
- `DistanceRatio`: Driver distance ratio (0.0-1.0)

## Penalty Calculation Rules

| Driver Distance | Penalty % | Refund % |
|-----------------|-----------|----------|
| 0% (pickup)     | 0%        | 100%     |
| 25%             | 10-15%    | 85-90%   |
| 50% (midpoint)  | 30-40%    | 60-70%   |
| 75%             | 60-70%    | 30-40%   |
| 90%+            | 70-80%    | 20-30%   |

## Running tests

Run the test suite:

```bash
dotnet test Tests/Tests.csproj
```

Key test files:
- `Tests/Application/Features/Orders/Services/RefundPenaltyCalculationServiceTests.cs`
- `Tests/Application/Features/Orders/Consumers/OrderCanceledPaymentRefundConsumerTests.cs`

## Sample payload

```json
{
  "OrderId": 123,
  "UserId": 456,
  "Amount": 20.00,
  "Reason": ["driver_too_far"],
  "Description": "Cancelamento após aceitação do motorista",
  "CancellationTime": "2026-05-17T12:34:56Z"
}
```

## Validation path

- The consumer validates the order exists and is canceled
- If the driver never accepted the order, penalty stays at 0
- If the driver accepted and approached the customer, the service calculates penalty up to 80%
- The result is logged for audit and future refund execution

## Current Limitations

1. Distance ratio calculation is currently a placeholder (returns 0.5)
2. Future iterations should use actual driver geolocation at acceptance time
3. No actual refund execution in this phase
