# Feature Specification: Database Migration Validation

**Feature Branch**: `006-database-migration-validation`  
**Created**: May 18, 2026  
**Status**: Draft  
**Input**: User description: "Realizamos diversas alterações ultimamente, desde a spec 003. Precisamos validar as mudanças que impactam o banco de dados, como novos campos e mudanças nos existentes. Vamos analisar o projeto, e planejar ações para que possamos executar uma migration com as últimas alterações (se houveram)."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Validate Subscription Entity Changes (Priority: P1)

Verify that the Subscription entity changes from spec 002 (unify subscriptions) are properly reflected in the database schema, including new columns for Asaas integration and SubscriptionType.

**Why this priority**: The Subscription entity is critical for the payment flow and the Asaas integration. Missing columns would break subscription creation and payment processing.

**Independent Test**: Can be tested by comparing the domain entity properties with the database schema and verifying all columns exist with correct data types.

**Acceptance Scenarios**:

1. **Given** the Subscription entity has `SubscriptionType`, `AsaasPaymentId`, `AsaasPaymentLink`, `AsaasSubscriptionId`, and `PaidAt` properties, **When** the database schema is inspected, **Then** all these columns should exist in the subscriptions table.
2. **Given** the SubscriptionType column, **When** the schema is checked, **Then** it should have a default value of 1 (Driver) for backward compatibility.
3. **Given** the AsaasPaymentId column, **When** the schema is checked, **Then** it should be nullable with max length 255.

---

### User Story 2 - Validate New Tables Exist (Priority: P1)

Verify that the DriverTransfers and Payments tables created in recent migrations are properly configured and accessible.

**Why this priority**: These tables are essential for the refund and transfer flows. Without them, the payment processing would fail.

**Independent Test**: Can be tested by querying the database for the existence of these tables and their columns.

**Acceptance Scenarios**:

1. **Given** the DriverTransfers migration (20260509164810), **When** the database is inspected, **Then** the DriverTransfers table should exist with OrderId, DriverId, Amount, Status, AsaasTransferId columns.
2. **Given** the Payments migration (20260509175600), **When** the database is inspected, **Then** the Payments table should exist with UserId, ReceiverId, Amount, Status, Type, AsaasPaymentId, PixQrCode, PixLink columns.

---

### User Story 3 - Validate RefundTransaction Migration (Priority: P1)

Verify that the RefundTransaction entity has a corresponding database migration. This entity exists in the domain but may be missing from the database.

**Why this priority**: The RefundTransaction entity is used for tracking PIX refund operations but has no migration file. This is a critical gap that must be addressed.

**Independent Test**: Can be tested by checking if a migration exists for RefundTransaction and if the table exists in the database.

**Acceptance Scenarios**:

1. **Given** the RefundTransaction entity exists in the domain, **When** migrations are listed, **Then** a migration for RefundTransaction table should exist.
2. **Given** the RefundTransaction entity has TransactionId, OrderId, Amount, CustomerWalletId, Status, RetryCount, ErrorDetails, AsaasTransferId properties, **When** the migration is created, **Then** all these columns should be present in the refund_transactions table.

---

### User Story 4 - Validate Order Entity Changes (Priority: P2)

Verify that the Order entity changes from spec 003 (cancel order flow) are properly reflected in the database schema.

**Why this priority**: The cancellation flow requires the CancelledBy, CancelDescription, and CancelRasons columns to function correctly.

**Independent Test**: Can be tested by checking the orders table schema against the entity definition.

**Acceptance Scenarios**:

1. **Given** the Order entity has CancelledBy, CancelDescription, CancelRasons properties, **When** the database schema is inspected, **Then** these columns should exist in the orders table.
2. **Given** the CancelledBy column, **When** the schema is checked, **Then** it should be nullable and store the cancellation initiator.

---

### Edge Cases

- What happens if a migration was created but not applied to the database?
- How does the system handle missing columns during runtime?
- What if there are pending model changes that haven't been migrated yet?
- What happens if the RefundTransaction table doesn't exist but code tries to use it?

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST verify that the subscriptions table has columns: SubscriptionType, AsaasPaymentId, AsaasPaymentLink, AsaasSubscriptionId, PaidAt.
- **FR-002**: System MUST verify that the driver_transfers table exists with all required columns from migration 20260509164810.
- **FR-003**: System MUST verify that the payments table exists with all required columns from migration 20260509175600.
- **FR-004**: System MUST create a new migration for the RefundTransaction entity if one does not exist.
- **FR-005**: System MUST verify that the orders table has CancelledBy, CancelDescription, CancelRasons columns.
- **FR-006**: System MUST run the migration against the database to apply any pending changes.
- **FR-007**: System MUST update the TILTContextModelSnapshot after migrations are applied.
- **FR-008**: System MUST document any discrepancies found between domain entities and database schema.

### Key Entities *(include if feature involves data)*

- **Subscription**: Unified subscription entity with Asaas integration fields. Key attributes: UserId, PlanId, Status, SubscriptionType, DueDate, PaymentToken, AsaasPaymentId, AsaasPaymentLink, AsaasSubscriptionId, PaidAt.
- **DriverTransfer**: Represents a transfer to a driver. Attributes: OrderId, DriverId, Amount, Status, AsaasTransferId, CompletedAt, FailedAt, ErrorMessage.
- **Payment**: Represents a payment transaction. Attributes: UserId, ReceiverId, Amount, Status, Type, ApprovedAt, CancelledAt, OrderId, AsaasPaymentId, PixQrCode, PixLink.
- **RefundTransaction**: Represents a PIX refund operation. Attributes: TransactionId, OrderId, Amount, CustomerWalletId, Status, RetryCount, ErrorDetails, AsaasTransferId.
- **Order**: Ride order entity with cancellation tracking. Attributes: Status, CancelledBy, CancelDescription, CancelRasons, CancelationTime.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: All domain entity properties have corresponding database columns with matching data types.
- **SC-002**: All migrations from spec 002, 003, 004, and 005 are applied to the database.
- **SC-003**: The RefundTransaction table is created and accessible.
- **SC-004**: No compilation errors occur after migration validation.
- **SC-005**: The TILTContextModelSnapshot matches the current database schema.

## Assumptions

- The database is PostgreSQL with PostGIS extension.
- Entity Framework Core is used for migrations.
- The hi-lo sequence pattern is used for ID generation.
- The "tilt" schema is the default for all tables.
- Existing data should not be lost during migration.
- The RefundTransaction entity was created but the migration was never generated.