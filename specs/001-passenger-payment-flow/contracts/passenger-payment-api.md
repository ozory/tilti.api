# Passenger Payment API Contracts

**Date**: May 3, 2026  
**Feature**: [spec.md](../spec.md)  
**Plan**: [plan.md](../plan.md)

## API Endpoints

### 1. Price Order (Existing)

**Endpoint**: `POST /api/orders/precify`  
**Handler**: `PrecifyOrderCommandHandler` (reuse existing)

**Request**:
```json
{
  "userId": 123,
  "addresses": [
    {
      "type": "origin",
      "street": "Rua Example, 123",
      "city": "São Paulo",
      "state": "SP",
      "zipCode": "01234-567"
    },
    {
      "type": "destination",
      "street": "Av. Paulista, 456",
      "city": "São Paulo",
      "state": "SP",
      "zipCode": "01310-100"
    }
  ]
}
```

**Response** (200 OK):
```json
{
  "id": 0,  // Temporary, order not created yet
  "userId": 123,
  "amount": {
    "value": 25.50,
    "currency": "BRL"
  },
  "distanceInKM": 8.5,
  "durationInSeconds": 1200,
  "status": "PendingPayment",
  "addresses": [...],
  "requestedTime": "2026-05-03T14:30:00Z"
}
```

---

### 2. Create Passenger Payment (New)

**Endpoint**: `POST /api/payments/passenger/create`  
**Handler**: `CreatePassengerPaymentCommandHandler` (new)

**Request**:
```json
{
  "userId": 123,
  "amount": 25.50,
  "externalReference": "order-temp-456",  // Temporary reference
  "dueDate": "2026-05-03T15:00:00Z"      // 15-30 min expiry
}
```

**Response** (200 OK):
```json
{
  "paymentId": "pay_123456789",
  "pixLink": "https://sandbox.asaas.com/payment-link/abc123",
  "qrCode": "data:image/png;base64,iVBORw0KGgo...",  // PIX QR code
  "amount": 25.50,
  "status": "PENDING",
  "dueDate": "2026-05-03T15:00:00Z"
}
```

**Error Responses**:
- 400 Bad Request: Validation errors
- 404 Not Found: User not found
- 500 Internal Server Error: Asaas API failure

---

### 3. Webhook - Payment Confirmation (New)

**Endpoint**: `POST /api/webhooks/asaas/passenger-ride`  
**Handler**: Add to `WebhookEndpoint.cs`

**Request** (from Asaas):
```json
{
  "event": "PAYMENT_CONFIRMED",
  "payment": {
    "id": "pay_123456789",
    "externalReference": "order-temp-456",
    "status": "CONFIRMED",
    "value": 25.50,
    "billingType": "PIX",
    "confirmedDate": "2026-05-03T14:35:00Z"
  }
}
```

**Response** (200 OK):
```json
{
  "message": "Webhook processed successfully",
  "orderId": 789,  // Created order ID
  "status": "ReadyToAccept"
}
```

**Events Triggered**:
- Calls `CreateOrderCommand` with payment confirmation
- Order created with status `ReadyToAccept`
- Payment entity updated to `Approved`

---

### 4. Create Order After Payment (Modified)

**Endpoint**: `POST /api/orders/create`  
**Handler**: `CreateOrderCommandHandler` (modified)

**Request**:
```json
{
  "userId": 123,
  "addresses": [...],
  "requestedTime": "2026-05-03T14:30:00Z",
  "amount": 25.50,
  "distanceInKM": 8.5,
  "durationInSeconds": 1200,
  "asaasPaymentId": "pay_123456789"  // NEW - Required
}
```

**Response** (200 OK):
```json
{
  "id": 789,
  "userId": 123,
  "amount": {
    "value": 25.50,
    "currency": "BRL"
  },
  "status": "ReadyToAccept",  // Changed from PendingPayment
  "addresses": [...],
  "requestedTime": "2026-05-03T14:30:00Z",
  "createdAt": "2026-05-03T14:35:00Z"
}
```

**Error Responses**:
- 400 Bad Request: Payment not confirmed
- 400 Bad Request: User already has open order
- 404 Not Found: Payment not found

---

## Data Contracts (C#)

### PaymentResponse
```csharp
namespace Application.Features.Payments.Contracts;

public class PaymentResponse
{
    public string PaymentId { get; set; }      // Asaas payment ID
    public string? PixLink { get; set; }        // PIX link for frontend
    public string? QrCode { get; set; }         // PIX QR code (base64)
    public decimal Amount { get; set; }
    public string Status { get; set; }          // "PENDING", "CONFIRMED", etc.
    public DateTime DueDate { get; set; }
}
```

### CreatePassengerPaymentCommand
```csharp
namespace Application.Features.Payments.Commands.CreatePassengerPayment;

public record CreatePassengerPaymentCommand(
    long UserId,
    decimal Amount,
    string ExternalReference,
    DateTime DueDate
) : ICommand<CreatePassengerPaymentCommand, PaymentResponse>;
```

---

## Flow Diagram

```
[Passenger]                [API]                    [Asaas]
    │                         │                         │
    ├─ 1. Price Order ────► │                         │
    │                         ├─ Calculate Price       │
    │                         └─ Return Estimate ◄────┤
    │                         │                         │
    ├─ 2. Create Payment ─► │                         │
    │                         ├─ Check/Create Wallet   │
    │                         ├─ POST /v3/paymentLinks ─►│
    │                         ├─ Return PIX Link ◄──────┤
    │                         │                         │
    │ 3. Display PIX Code    │                         │
    │    (QR Code/Link)      │                         │
    │                         │                         │
    │ 4. Pay via PIX ───────┼────────────────────────►│
    │                         │                         │
    │                         │   5. Webhook ──────────►│
    │                         ├─ POST /api/webhooks/...─►│
    │                         ├─ Verify Payment        │
    │                         ├─ Create Order          │
    │                         └─ Return Order ◄────────┤
    │                         │                         │
    ├─ 6. Order Ready ◄─────┤                         │
    │   (ReadyToAccept)       │                         │
```

---

## Security Considerations

1. **Authentication**: All endpoints require authenticated user (JWT)
2. **Authorization**: User can only create payments for themselves
3. **Payment Validation**: Verify payment belongs to requesting user
4. **Webhook Security**: Validate webhook signature from Asaas (if available)
5. **Idempotency**: Handle duplicate webhooks gracefully

---

## Testing Contracts

### Success Scenario
1. Price order → Get estimate
2. Create payment → Get PIX link
3. Simulate payment → Webhook called
4. Verify order created → Status = `ReadyToAccept`

### Failure Scenarios
1. Payment expired → Order not created
2. Payment insufficient → Order not created
3. Webhook invalid → Return 400, log error
4. User already has order → Return 400
