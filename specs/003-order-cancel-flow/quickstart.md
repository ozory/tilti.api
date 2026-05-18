# Quickstart: Order Cancellation Flow

## Overview

This quickstart guide shows how to use the order cancellation feature.

## Prerequisites

- Order must exist in the system
- User must be authenticated
- Order status must not be `InTransit`, `Finished`, or `Canceled`

## API Usage

### Cancel an Order

```bash
curl -X POST "https://api.tilt.com/api/orders/{orderId}/cancel" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "userId": 123,
    "requestedTime": "2026-05-17T10:30:00Z",
    "reason": ["changed_mind"],
    "description": "Customer changed their mind",
    "cancelledBy": "User"
  }'
```

### Response

**Success (200 OK):**
```json
{
  "id": 123,
  "status": "Canceled",
  "cancelledBy": "User",
  "cancelDescription": "Customer changed their mind",
  "cancelRasons": "changed_mind",
  "cancelationTime": "2026-05-17T10:30:00Z"
}
```

**Error (400 Bad Request):**
```json
{
  "errors": ["Não é possível cancelar um pedido em andamento"]
}
```

## Implementation Reference

| Component | Location |
|-----------|----------|
| Command | `Application/Features/Orders/Commands/Cancel/CancelOrderCommand.cs` |
| Handler | `Application/Features/Orders/Commands/Cancel/CancelOrderCommandHandler.cs` |
| Validator | `Application/Features/Orders/Commands/Cancel/CancelOrderCommandValidator.cs` |
| Endpoint | `Api/Endpoints/OrdersEndpoint.cs` |
| Tests | `Tests/Application/Features/Orders/Commands/CancelOrderCommandHandlerTests.cs` |

## Running Tests

```bash
# Run all cancellation tests
dotnet test Tests/Tests.csproj --filter "CancelOrder"

# Run specific test class
dotnet test Tests/Tests.csproj --filter "CancelOrderCommandHandlerTests"
```

## Configuration

Ensure the following is configured in `appsettings.json`:

```json
{
  "Infrastructure": {
    "OrderCanceledPaymentRefundMessages": {
      "exchange": "tilt.order.canceled.payment.refund.exchange",
      "exchangeType": "topic",
      "queue": "tilt.order.canceled.payment.refund.queue",
      "routingKey": "tilt.order.canceled.payment.refund"
    }
  }
}
```