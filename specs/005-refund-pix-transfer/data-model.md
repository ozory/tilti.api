# Data Model: Refund via PIX Transfer

**Date**: 2026-05-17
**Spec**: [spec.md](spec.md)

## Entities

### RefundTransaction

Represents a PIX refund operation for a canceled order.

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| TransactionId | Guid | Yes | Unique identifier for the refund transaction |
| OrderId | long | Yes | Reference to the canceled order |
| Amount | decimal | Yes | Refund amount in BRL |
| CustomerWalletId | string | Yes | Customer's PIX key (random UUID format) |
| Status | RefundTransactionStatus | Yes | Current status of the transaction |
| CreatedAt | DateTime | Yes | Timestamp when transaction was created |
| UpdatedAt | DateTime | Yes | Timestamp of last status update |
| RetryCount | int | Yes | Number of retry attempts made |
| ErrorDetails | string | No | Error message if transaction failed |
| AsaasTransferId | string | No | External transaction ID from Asaas |

#### RefundTransactionStatus Enum

```csharp
public enum RefundTransactionStatus
{
    Pending,    // Initial state, transfer not yet attempted
    Completed,  // Transfer successful
    Failed      // Transfer failed after all retries
}
```

### Validation Rules

1. **Amount**: Must be greater than 0 (zero-amount refunds skip PIX transfer)
2. **CustomerWalletId**: Must be a valid UUID format
3. **Status transitions**: Pending → Completed, Pending → Failed (after retries exhausted)

## Relationships

```
RefundTransaction
├── Order (1:1) - Each refund belongs to one order
└── User (1:1) - Each refund belongs to one customer (via WalletId)
```

## MongoDB Collection

- **Collection Name**: `refund_transactions`
- **Indexes**:
  - `TransactionId` (unique)
  - `OrderId` (indexed for query performance)
  - `Status` (indexed for monitoring)
  - `CreatedAt` (indexed for time-based queries)

## Domain Events

### RefundPixTransferCompletedEvent

| Field | Type | Description |
|-------|------|-------------|
| TransactionId | Guid | Refund transaction identifier |
| OrderId | long | Order that was canceled |
| Amount | decimal | Refunded amount |
| CustomerWalletId | string | Customer's PIX key |
| Timestamp | DateTime | When the transfer completed |

### RefundPixTransferFailedEvent

| Field | Type | Description |
|-------|------|-------------|
| TransactionId | Guid | Refund transaction identifier |
| OrderId | long | Order that was canceled |
| ErrorDetails | string | Error message from failed transfer |
| RetryCount | int | Number of attempts made |
| Timestamp | DateTime | When the failure was recorded |