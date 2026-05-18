# Feature Specification: Refund via PIX Transfer

**Feature Branch**: `[###-refund-pix-transfer]`
**Created**: 2026-05-17
**Status**: Draft
**Input**: User description: "Agora que já podemos calcular o reembolso quando há o cancelamento pelo cliente, vamos realizar a transação de reembolso da walledId da Tilt para a WalletId do cliente via PIX"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Refund via PIX after client cancellation (Priority: P1)

When a client cancels an order, the system calculates the refund amount (including any penalty) and automatically transfers the refund from Tilt's wallet to the customer's WalletId using PIX.

**Why this priority**: This is the core business value – delivering the promised refund to the client quickly and securely.

**Independent Test**: Cancel an order as a client, verify that a PIX transfer is initiated, and confirm that the customer's bank account receives the correct amount.

**Acceptance Scenarios**:

1. **Given** an order canceled by the client with a positive refund amount, **When** the refund process runs, **Then** a PIX transfer is created and the amount is credited to the customer's WalletId.
2. **Given** an order canceled by the client where the refund amount is zero, **When** the refund process runs, **Then** no PIX transfer is performed.

---

### User Story 2 - Handle invalid or missing WalletId (Priority: P2)

If the customer's WalletId (PIX key) is missing or invalid, the system must not attempt a transfer and must log the issue for manual handling.

**Why this priority**: Prevents failed transfers and potential financial loss.

**Independent Test**: Cancel an order with an invalid WalletId and verify that the system logs an error and does not attempt a PIX transfer.

**Acceptance Scenarios**:

1. **Given** an order canceled by the client with an invalid WalletId, **When** the refund process runs, **Then** the system logs a validation error and skips the PIX transfer.

---

### User Story 3 - Retry on transient PIX failures (Priority: P3)

When a PIX transfer fails due to a transient error (e.g., network issue), the system retries up to three times before marking the refund as failed.

**Why this priority**: Improves reliability and reduces manual intervention.

**Independent Test**: Simulate a transient failure on the first attempt and verify that the system retries and eventually succeeds.

**Acceptance Scenarios**:

1. **Given** a transient failure on the first PIX attempt, **When** the retry logic executes, **Then** the transfer succeeds on a subsequent attempt and the transaction is marked as completed.
2. **Given** persistent failures after three attempts, **When** retries are exhausted, **Then** the transaction is marked as failed and an alert is logged.

## Edge Cases

- What happens when Tilt's wallet balance is insufficient for the refund?
- How does the system handle a PIX transfer that succeeds but the confirmation is delayed?
- What if the customer’s bank rejects the PIX transfer after it was sent?

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST initiate a PIX transfer from Tilt's wallet to the customer's WalletId for the calculated refund amount when an order is canceled by the client.
- **FR-002**: System MUST validate that the customer's WalletId is a syntactically valid PIX key before initiating the transfer.
- **FR-003**: System MUST retry a failed PIX transfer up to three times with exponential back‑off before marking the transaction as failed.
- **FR-004**: System MUST record each refund transaction (TransactionId, OrderId, Amount, Status, Timestamps) in an audit log accessible to operations.
- **FR-005**: System MUST not initiate a PIX transfer if the calculated refund amount is zero or if the order is not in a client‑canceled state.
- **FR-006**: System MUST ensure that the refund transaction is only started after the penalty calculation service has successfully completed.
- **FR-007**: System MUST emit an event `RefundPixTransferCompletedEvent` with details of the successful transfer for downstream notifications.
- **FR-008**: System MUST emit an event `RefundPixTransferFailedEvent` after exhausting retries, including error details.

### Key Entities *(include if feature involves data)*

- **RefundTransaction**: Represents a PIX refund operation. Attributes: TransactionId, OrderId, Amount, CustomerWalletId, Status (Pending, Completed, Failed), CreatedAt, UpdatedAt, RetryCount.
- **Wallet**: Represents a financial wallet. Attributes: WalletId (Tilt's internal ID), Balance.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 95% of refund PIX transfers complete successfully within 2 minutes of order cancellation.
- **SC-002**: No more than 1% of refunds remain in a failed state after all retries.
- **SC-003**: 99% of customers receive the refunded amount in their bank account within 24 hours.
- **SC-004**: Audit logs contain a complete record for 100% of refund transactions, including timestamps and status.

## Assumptions

- Tilt has an existing, configured PIX integration service that can be called synchronously.
- Customers provide a valid PIX key (WalletId) during account creation; the key is stored in the user profile.
- The penalty calculation service already returns the final refundable amount.
- Tilt's wallet has sufficient balance; insufficient‑balance scenarios will be logged and escalated manually.
- Transfer fees are covered by Tilt and do not affect the refundable amount.
- The background consumer that processes `OrderCanceledPaymentRefundDomainEvent` will be extended to invoke the PIX service after penalty calculation.

## Clarifications

### Session 2026-05-17

- **Q:** Which type of WalletId (PIX key) should be used for the refund transfer?
	**A:** Random UUID (system‑generated) key.
