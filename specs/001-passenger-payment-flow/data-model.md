# Data Model: Passenger Travel Payment Flow

**Date**: May 3, 2026  
**Feature**: [spec.md](../spec.md)  
**Plan**: [plan.md](../plan.md)

## Entities

### Order (Existing - Modified)

**Location**: `Domain/Features/Orders/Entities/Order.cs`

**Existing Properties**:
- `Id` (long)
- `UserId` (long) - Passenger ID
- `DriverId` (long?) - Assigned driver (null initially)
- `Amount` (Amount) - Total price
- `Status` (OrderStatus) - **Will transition from `PendingPayment` → `ReadyToAccept`**
- `DistanceInKM` (int)
- `DurationInSeconds` (int)
- `_addresses` (List<Address>) - Origin and destination
- `RequestedTime` (DateTime?)
- `CreatedAt` (DateTime?)

**New Properties**:
- `PaymentId` (string?) - Links to Asaas payment ID (external reference)

**State Transitions**:
```
[Initial] → PendingPayment (after pricing)
    ↓ (payment confirmed)
ReadyToAccept (available for driver matching)
    ↓ (driver accepts)
Accepted → InTransit → Finished
```

**Validation Rules**:
- Amount must be > 0
- UserId must be valid (passenger exists)
- Addresses must have at least origin and destination
- Status can only transition: `PendingPayment` → `ReadyToAccept` (after payment)

---

### Payment (Existing - Modified)

**Location**: `Domain/Features/Payments/Entities/Payment.cs`

**Existing Properties**:
- `Id` (long)
- `Identifier` (Guid?) - Internal identifier
- `UserId` (long) - Payer (passenger)
- `ReceiverId` (long) - Receiver (passenger, or platform)
- `Amount` (Amount)
- `Status` (PaymentStatus) - `Pending` → `Approved` or `Cancelled`
- `Type` (PaymentType) - **Add `PassengerRide = 3`**
- `ApprovedAt` (DateTime?)
- `CancelledAt` (DateTime?)

**New Properties**:
- `OrderId` (long?) - Links payment to order (set after order creation)
- `AsaasPaymentId` (string?) - Asaas payment ID for webhook correlation
- `PixQrCode` (string?) - PIX QR code returned by Asaas
- `PixLink` (string?) - PIX link for frontend display

**Validation Rules**:
- Amount must match Order.Amount
- UserId must be valid passenger
- Status transitions: `Pending` → `Approved` (on webhook confirmation) OR `Pending` → `Cancelled` (on failure/expiry)

---

### User (Existing - Reused)

**Location**: `Domain/Features/Users/Entities/User.cs`

**Relevant Existing Properties**:
- `Id` (long)
- `AsaasWalletId` (string?) - **Will be populated by wallet creation flow**
- `Name` (PersonName)
- `Document` (Document) - CPF for wallet creation
- `Email` (Email)

**No modifications needed** - wallet ID property already exists.

---

## Enums

### OrderStatus (Existing - No changes)

**Location**: `Domain/Features/Orders/Enums/OrderStatus.cs`

```csharp
public enum OrderStatus : ushort
{
    PendingPayment = 1,    // Order priced, awaiting payment
    ReadyToAccept = 2,      // Payment confirmed, ready for driver
    Accepted = 3,
    InTransit = 4,
    Finished = 5,
    Canceled = 6,
    Expired = 7,
}
```

### PaymentStatus (Existing - No changes)

**Location**: `Domain/Features/Payments/Enums/PaymentStatus.cs` (to verify)

```csharp
public enum PaymentStatus : ushort
{
    Pending = 1,
    Approved = 2,
    Cancelled = 3,
    // May need: Expired = 4
}
```

### PaymentType (Existing - Add PassengerRide)

**Location**: `Domain/Features/Payments/Enums/PaymentType.cs` (to verify)

**Current** (assumed):
```csharp
public enum PaymentType : ushort
{
    CreditCard = 1,
    PIX = 2,
    // Add:
    PassengerRide = 3  // NEW - For passenger ride payments
}
```

---

## Relationships

```
User (Passenger)
  │
  │ Has wallet: User.AsaasWalletId → Asaas Wallet
  │
  ├─→ Payment (UserId) [1:N]
  │     │
  │     │ Type = PassengerRide
  │     │ Status: Pending → Approved
  │     │ AsaasPaymentId: Links to Asaas
  │     │
  │     └─→ Order (PaymentId) [1:1]
  │           │
  │           │ Status: PendingPayment → ReadyToAccept
  │           │ UserId: Same passenger
  │           │ Amount: Must match Payment.Amount
  │           │
  │           └─→ (Driver assigned later)
  │
  └─→ (Other payments: subscriptions, etc.)
```

---

## Database Considerations

### New/Modified Tables

**Payments** (Existing - Add columns):
- `OrderId` (bigint, nullable, FK to Orders.Id)
- `AsaasPaymentId` (varchar(100), nullable)
- `PixQrCode` (text, nullable)
- `PixLink` (text, nullable)

**Orders** (Existing - Add columns):
- `PaymentId` (varchar(100), nullable) - Asaas payment ID

### Migrations Required

1. Add new columns to `Payments` table
2. Add new columns to `Orders` table
3. Add index on `Payments.AsaasPaymentId` for webhook lookup
4. Add index on `Orders.PaymentId` for correlation

---

## Contracts (API Layer)

### CreatePassengerPaymentCommand

**Location**: `Application/Features/Payments/Commands/CreatePassengerPayment/`

```csharp
public record CreatePassengerPaymentCommand(
    long UserId,
    decimal Amount,
    string OrderExternalReference,  // Will be Order.Id.ToString()
    DateTime DueDate                 // Payment expiry (15-30 min from now)
) : IRequest<Result<PaymentResponse>>;
```

### PaymentResponse

**Location**: `Application/Features/Payments/Contracts/`

```csharp
public class PaymentResponse
{
    public string PaymentId { get; set; }      // Asaas payment ID
    public string? PixLink { get; set; }        // PIX link for frontend
    public string? PixQrCode { get; set; }      // PIX QR code (base64 or SVG)
    public decimal Amount { get; set; }
    public string Status { get; set; }          // "PENDING", "CONFIRMED", etc.
    public DateTime DueDate { get; set; }
}
```

### Modified CreateOrderCommand

**Location**: `Application/Features/Orders/Commands/Create/`

**Add parameter**:
```csharp
public record CreateOrderCommand(
    // ... existing parameters ...
    string AsaasPaymentId  // NEW - Required for payment validation
) : IRequest<Result<OrderResponse>>;
```

**Handler modification**: Validate payment exists and is approved before creating order.

---

## Validation Rules Summary

| Entity | Field | Rule |
|--------|-------|------|
| Order | Status | Can only transition `PendingPayment` → `ReadyToAccept` after payment |
| Order | PaymentId | Must be non-null when transitioning to `ReadyToAccept` |
| Order | Amount | Must match Payment.Amount |
| Payment | Type | Must be `PassengerRide` for ride payments |
| Payment | Status | Must be `Approved` before creating Order |
| Payment | AsaasPaymentId | Must be non-null (set by Asaas) |
| User | AsaasWalletId | Must be non-null before payment (create wallet if needed) |
