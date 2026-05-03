# Research: Passenger Travel Payment Flow

**Date**: May 3, 2026  
**Feature**: [spec.md](../spec.md)  
**Plan**: [plan.md](../plan.md)

## Research Questions & Decisions

### 1. How to create wallets in Asaas for passengers?

**Decision**: Use same pattern as `DriverPaymentService.CreateDriverWalletAsync()`

**Rationale**:
- Project already has working wallet creation for drivers
- Uses POST `/v3/accounts` endpoint
- Pattern: Check if wallet exists → Create if not → Save wallet ID to User entity

**Implementation**:
```csharp
// From DriverPaymentService.cs (lines 145-188)
if (!string.IsNullOrEmpty(user.AsaasWalletId))
    return user.AsaasWalletId;

// Create wallet via POST /v3/accounts
var walletRequest = new WalletRequest(
    name: user.Name.Value,
    cpfCnpj: user.Document.Value,
    email: user.Email.Value,
    ...
);

// Save wallet ID
user.SetAsaasWalletId(walletResponse.id);
await unitOfWork.UserRepository.UpdateAsync(user);
await unitOfWork.CommitAsync(cancellationToken);
```

**Alternatives considered**:
- Create wallet only when payment fails (rejected because wallet check is cheap, creation is expensive)
- Use separate wallet service (rejected because driver pattern works well)

---

### 2. How to process PIX payments for passengers?

**Decision**: Create payment link via POST `/v3/paymentLinks` (same as driver subscriptions)

**Rationale**:
- Driver subscriptions already use payment links successfully
- Returns PIX code/link that frontend can display
- Asaas sends webhook when payment is confirmed

**Implementation** (from `DriverPaymentService.CreatePaymentLinkAsync()`):
```csharp
var paymentLinkRequest = new PaymentLinkRequest(
    name: $"Corrida - {order.Id}",
    billingType: "PIX",
    chargeType: "DETACHED",  // Single payment, not recurrent
    value: order.Amount.Value,
    description: $"Pagamento de corrida - Order {order.Id}",
    dueDate: DateTime.Now.AddMinutes(30),  // Short expiry for ride payments
    installmentCount: 1,
    active: true,
    externalReference: order.Id.ToString()
);
```

**Alternatives considered**:
- Direct payment via POST `/v3/payments` (rejected because payment links give PIX code immediately)
- Use credit card (rejected per requirement - passenger ride payment should be PIX)

---

### 3. How to link payments to orders?

**Decision**: Use `externalReference` field in Asaas to store Order ID

**Rationale**:
- Already used in driver subscriptions (`externalReference: driverSubscription.Id.ToString()`)
- Webhook can extract Order ID from `externalReference` field
- Simple and reliable linkage

**Implementation**:
- When creating payment: `externalReference: orderId.ToString()`
- In webhook handler: Parse `externalReference` → Get Order ID → Create/Update Order

---

### 4. When to create the Order - before or after payment?

**Decision**: Create Order AFTER payment confirmation (with status `PendingPayment` → `ReadyToAccept`)

**Rationale**:
- Requirement FR-006: "System MUST only create travel orders after successful payment confirmation"
- Order entity already has `PendingPayment` status (enum value = 1)
- Prevents fake orders and keeps database clean

**Flow**:
1. Passenger prices order (no DB write)
2. Passenger pays (create payment record with status `Pending`)
3. Asaas confirms payment → webhook
4. Webhook handler: Update payment status → Create Order with status `ReadyToAccept`

**Alternatives considered**:
- Create Order with `PendingPayment` status before payment (rejected per requirement)
- Create Order after payment in same transaction (rejected because webhook is asynchronous)

---

### 5. How to handle payment expiration?

**Decision**: Set payment link expiry to 15-30 minutes, handle expired payments gracefully

**Rationale**:
- Ride pricing shouldn't be valid forever
- Short expiry encourages quick payment
- Edge case identified in spec

**Implementation**:
- Set `dueDate` to 15-30 minutes from creation
- If payment expires: Asaas webhook sends `PAYMENT_OVERDUE` event
- Handler: Mark payment as expired, notify passenger they need to re-price

---

## Summary of Technical Decisions

| Decision | Choice | Reason |
|-----------|--------|--------|
| Wallet creation | Reuse `DriverPaymentService` pattern | Proven pattern, already working |
| Payment method | PIX via payment links | Matches driver subscriptions, gives PIX code |
| Order-Payment linkage | `externalReference` field | Already used in subscriptions |
| Order creation timing | After payment confirmation | Requirement FR-006 |
| Payment expiry | 15-30 minutes | Ride pricing shouldn't be permanent |

## API Contracts Research

### Asaas Payment Link Request (from existing code)
```csharp
public record PaymentLinkRequest(
    string name,
    string billingType,      // "PIX"
    string chargeType,       // "DETACHED" for single payment
    decimal value,
    string description,
    DateTime dueDate,
    int installmentCount,
    bool active,
    string externalReference
);
```

### Asaas Payment Link Response (from existing code)
```csharp
public record PaymentLinkResponse(
    string id,
    string url,          // PIX link for frontend
    string qrCode        // PIX QR code (if available)
);
```

## References

- `Infrastructure/External/Features/Payments/Services/DriverPaymentService.cs` - Wallet & payment patterns
- `Application/Features/Subscriptions/Commands/CreateDriverSubscription/` - Payment link creation
- `Api/Endpoints/WebhookEndpoint.cs` - Webhook handling pattern
- `Domain/Features/Orders/Enums/OrderStatus.cs` - Order statuses
- `Domain/Features/Payments/Entities/Payment.cs` - Payment entity
