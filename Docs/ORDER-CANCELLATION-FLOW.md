# Order Cancellation Flow

## Overview

This document describes the order cancellation feature implementation for the Tilt API.

## Endpoint

**POST** `/api/orders/{orderId}/cancel`

### Description
Cancels an order if it's not already in progress. Only the user who placed the order (or the assigned driver) can cancel it.

### Path Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| orderId   | long | The unique identifier of the order to cancel |

### Request Body

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| UserId | long | Yes | The ID of the user requesting cancellation |
| RequestedTime | DateTime | Yes | Timestamp when cancellation was requested |
| reason | List<string> | No | List of cancellation reason tags |
| description | string | No | Detailed description of cancellation reason |
| cancelledBy | string | No | Who initiated cancellation ("User" or "Driver"). Defaults to "User" if not provided |

### Responses

#### Success Response
- **Code**: 200 OK
- **Content**: OrderResponse object representing the cancelled order

#### Error Responses
- **Code**: 400 Bad Request
  - "Nenhum pedido encontrado" (Order not found)
  - "Não é possível cancelar um pedido em andamento" (Cannot cancel an order in progress)
  - "Pedido já foi cancelado" (Order already cancelled)
  - "Pedido já foi Finalizado" (Order already finished)
  - "Usuário não autorizado a cancelar este pedido" (User not authorized)
  - "Motorista não autorizado a cancelar este pedido" (Driver not authorized)

---

## Implementation Details

### Files

| File | Purpose |
|------|---------|
| `Application/Features/Orders/Commands/Cancel/CancelOrderCommand.cs` | Command definition |
| `Application/Features/Orders/Commands/Cancel/CancelOrderCommandValidator.cs` | Command validation |
| `Application/Features/Orders/Commands/Cancel/CancelOrderCommandHandler.cs` | Command handler |
| `Api/Endpoints/OrdersEndpoint.cs` | API endpoint mapping |
| `Domain/Features/Orders/Entities/Order.cs` | Order entity with cancellation fields |
| `Domain/Features/Orders/Events/OrderCanceledPaymentRefundDomainEvent.cs` | Domain event for refund |

### Status Transitions

```
PendingPayment → Canceled (allowed)
ReadyToAccept → Canceled (allowed)
Accepted → Canceled (allowed)
InTransit → Canceled (NOT allowed - returns error)
Finished → Canceled (NOT allowed - returns error)
Canceled → Canceled (NOT allowed - returns error)
```

### Cancellation Initiator

The system tracks who initiated the cancellation:

- **User**: When a passenger cancels before driver acceptance
  - Publishes `OrderCanceledPaymentRefund` event to RabbitMQ for refund processing
  
- **Driver**: When a driver cancels after accepting the ride
  - No refund event is published

---

## RabbitMQ Event

When a user successfully cancels an order, the system publishes an `OrderCanceledPaymentRefund` event:

**Exchange**: `tilt.order.canceled.payment.refund.exchange`  
**Routing Key**: `tilt.order.canceled.payment.refund`  
**Queue**: `tilt.order.canceled.payment.refund.queue`

**Event Payload**:
```json
{
  "OrderId": 123,
  "UserId": 456,
  "Amount": 25.50,
  "Reason": ["user_request"],
  "Description": "Customer changed mind",
  "CancellationTime": "2026-05-17T10:30:00Z"
}
```

---

## Refund Penalty Consumer

The `OrderCanceledPaymentRefundConsumer` processes the cancellation event and calculates the refund penalty:

### Implementation
- **Consumer**: `Application/Features/Orders/Consumers/OrderCanceledPaymentRefundConsumer.cs`
- **Service**: `Application/Features/Orders/Services/RefundPenaltyCalculationService.cs`
- **Value Object**: `Domain/Features/Orders/ValueObjects/PenaltyCalculationResult.cs`

### Penalty Rules
- **No driver acceptance**: 0% penalty (full refund)
- **Driver at 0% distance**: 0% penalty
- **Driver at 50% distance**: 30-40% penalty
- **Driver at 90%+ distance**: 70-80% penalty (capped)
- **Minimum refund**: 20% of original amount

### Audit Logging
All penalty calculations are logged with:
- OrderId
- Original amount
- Penalty percentage
- Refund amount
- Distance ratio

---

## Tests

Unit tests are located in:
- `Tests/Application/Features/Orders/Commands/CancelOrderCommandValidatorTests.cs`
- `Tests/Application/Features/Orders/Commands/CancelOrderCommandHandlerTests.cs`

### Running Tests

```bash
dotnet test Tests/Tests.csproj --filter "CancelOrder"
```

---

## Configuration

Add to `appsettings.json`:

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