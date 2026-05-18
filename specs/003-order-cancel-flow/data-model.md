# Data Model: Cancel Order Flow

**Date**: May 17, 2026  
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
- `Status` (OrderStatus) - Current order status
- `DistanceInKM` (int)
- `DurationInSeconds` (int)
- `_addresses` (List<Address>) - Origin and destination
- `RequestedTime` (DateTime?)
- `CreatedAt` (DateTime?)
- `CancelDescription` (string?) - Cancellation description
- `CancelRasons` (string?) - Cancellation reasons
- `CancelledBy` (string?) - Who cancelled the order (User/Driver)
- `CancelationTime` (DateTime?) - When the order was cancelled

**New Properties**: None (all required properties already exist)

**State Transitions for Cancellation**:
```
[Any status except InTransit] → Canceled (when user cancels before driver acceptance)
    ↓
[InTransit] → Cancellation not allowed (error returned)
```

**Validation Rules**:
- Cancellation only allowed when Status != OrderStatus.InTransit (InProgress in spec maps to InTransit in enum)
- When cancelled by user: CancelledBy must be set to "User"
- When cancelled by driver: CancelledBy must be set to "Driver"
- CancelDescription and CancelRasons should be provided for audit purposes
- CancelationTime should be set to current UTC time upon cancellation

### OrderCanceledPaymentRefund Message

**Location**: To be defined in messaging layer

**Properties**:
- `OrderId` (long) - The cancelled order identifier
- `UserId` (long) - The user who initiated cancellation
- `Amount` (decimal) - The amount to be refunded
- `Reason` (string) - Cancellation reason tag
- `Description` (string) - Detailed cancellation description
- `CancellationTime` (DateTime) - When cancellation occurred

**Usage**: Published to RabbitMQ when a user cancels an order (not when driver cancels)

## Related Enums

### OrderStatus (Existing)

**Location**: `Domain/Features/Orders/Enums/OrderStatus.cs`

```csharp
public enum OrderStatus : ushort
{
    PendingPayment = 1,
    ReadyToAccept = 2,
    Accepted = 3,
    InTransit = 4,      // Maps to "InProgress" in spec
    Finished = 5,
    Canceled = 6,       // Maps to "Cancelada" in spec
    Expired = 7,
}
```

## Database Schema Impact

### PostgreSQL Order Table (Existing - No Changes Needed)

Based on the existing OrderConfiguration, the table already has columns for:
- CancelDescription (varchar(500))
- CancelRasons (varchar(500))
- CancelledBy (implicitly exists as string property)
- CancelationTime (timestamp)

No schema changes are required as all necessary fields already exist in the entity and are mapped in the configuration.