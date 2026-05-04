# Feature Specification: Unify Subscriptions

**Feature Branch**: `002-unify-subscriptions`  
**Created**: 2026-05-03  
**Status**: Draft  
**Input**: User description: "Parece que as migrations de DriverSubscriptions não foram executadas. E nisso temos um problema: Há duas referências de subscriptions: DriverSubscriptions e Subscriptions, sendo que o certo, é apenas Subscriptions, pois até a tabela já existe no banco. Vamos desconsiderar a DriverSubscription de todo o projeto, mantendo apenas Subscription. Mas para não perdermos o que já temos, adicione as propriedades que estão a mais em DriverSubscriptions no domínio, na Subscription e no restante todo do projeto, faça a substituição quando necessário, de DriverSubscription para Subscription."
## Clarifications

### Session 2026-05-03

- Q: Who should the unified Subscription entity target? (Only drivers, both drivers and passengers, or other?) → A: Both drivers and passengers (Subscription becomes a generic entity for all user types)
- Q: How to differentiate between driver and passenger subscriptions in the unified model? → A: Add SubscriptionType enum (Driver, Passenger) to the Subscription entity
- Q: Can a user have multiple active subscriptions of the same type? → A: No, only one active subscription per user per type (Driver or Passenger)
- Q: How to handle Asaas payment integration failures? → A: Mark subscription as PaymentFailed and notify user/admin for manual retry
- Q: How to handle existing records when adding SubscriptionType column? → A: Default to "Driver" for existing records (assumed to be driver subscriptions)

---
## User Scenarios & Testing *(mandatory)*

### User Story 1 - Unify Subscription Model (Priority: P1)

Consolidate the subscription model by removing the duplicate DriverSubscription entity and keeping only the Subscription entity, ensuring all properties from DriverSubscription are preserved in the unified model.

**Why this priority**: This is a critical architectural improvement that eliminates duplication and confusion between two similar entities. The current state has both `Subscription` and `DriverSubscription` entities, but only the `subscriptions` table exists in the database. This unification will prevent future migration issues and simplify the codebase.

**Independent Test**: Can be fully tested by verifying that all Subscription-related operations (create, update, query) work correctly with the unified model, and that no references to DriverSubscription remain in the codebase.

**Acceptance Scenarios**:

1. **Given** the current codebase with both Subscription and DriverSubscription entities, **When** the unification is complete, **Then** only the Subscription entity should exist with all properties from DriverSubscription added
2. **Given** a developer working on the project, **When** they search for DriverSubscription references, **Then** no results should be found in the codebase
3. **Given** the database with the existing subscriptions table, **When** the application runs, **Then** it should use the unified Subscription entity without requiring new migrations

---

### User Story 2 - Update Application Layer (Priority: P2)

Update all application layer components (Commands, Queries, Handlers) to use the unified Subscription entity instead of DriverSubscription.

**Why this priority**: After unifying the domain model, all application layer components must be updated to work with the new unified model. This ensures the application functions correctly with the consolidated entity.

**Independent Test**: Can be tested by running all subscription-related use cases and verifying they work correctly with the Subscription entity.

**Acceptance Scenarios**:

1. **Given** the unified Subscription entity, **When** creating a new subscription, **Then** the CreateSubscriptionCommand should use Subscription instead of DriverSubscription
2. **Given** an existing subscription, **When** querying subscription details, **Then** the query handlers should return Subscription data
3. **Given** subscription activation/cancellation operations, **When** executed, **Then** they should work with the unified Subscription entity

---

### User Story 3 - Update Infrastructure Layer (Priority: P3)

Update the infrastructure layer including repositories, configurations, and database context to work with the unified Subscription entity.

**Why this priority**: The infrastructure layer needs to be updated to remove DriverSubscription references and ensure proper persistence of the unified Subscription entity.

**Independent Test**: Can be tested by verifying that database operations (save, update, query) work correctly with the Subscription entity and that the DbContext no longer references DriverSubscription.

**Acceptance Scenarios**:

1. **Given** the unified Subscription entity, **When** the application saves a subscription, **Then** it should persist to the existing subscriptions table
2. **Given** the database context, **When** queried for subscriptions, **Then** it should return Subscription entities (not DriverSubscription)
3. **Given** the repository implementation, **When** subscription queries are executed, **Then** they should use the unified Subscription entity

---

### Edge Cases

- What happens when existing code still references DriverSubscription after unification? (Should not compile)
- How does the system handle the absence of DriverSubscription table in the database? (Use existing subscriptions table, no driver_subscriptions table needed)
- What happens if there are pending migrations for DriverSubscription? (Should be removed/skipped)
- What if the existing subscriptions table doesn't have the new columns? (A new migration must be created and executed to add AsaasPaymentId, AsaasPaymentLink, AsaasSubscriptionId, PaidAt)
- What happens when Asaas payment fails? (Mark subscription as PaymentFailed and notify user/admin for manual retry)
- What if user has both Driver and Passenger subscriptions and one fails? (Handle independently per SubscriptionType)

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST remove the DriverSubscription entity from the Domain layer
- **FR-002**: System MUST add the following properties from DriverSubscription to Subscription entity: `AsaasPaymentId`, `AsaasPaymentLink`, `AsaasSubscriptionId`, `PaidAt`
- **FR-003**: System MUST update the Subscription entity's Create method to handle the new properties
- **FR-003.1**: System MUST enforce uniqueness constraint: only one active subscription per user per SubscriptionType (Driver or Passenger)
- **FR-004**: System MUST remove all DriverSubscription-related Commands and Queries from the Application layer
- **FR-005**: System MUST update all Command/Query handlers to use Subscription instead of DriverSubscription
- **FR-006**: System MUST remove the IDriverSubscriptionRepository interface and replace with ISubscriptionRepository (if not already comprehensive)
- **FR-007**: System MUST update the DriverSubscriptionRepository to work with Subscription entity or remove it
- **FR-008**: System MUST remove DriverSubscriptionConfiguration from Infrastructure layer
- **FR-009**: System MUST update TILTContext to remove DbSet<DriverSubscription> and ensure DbSet<Subscription> is properly configured
- **FR-010**: System MUST update all endpoints in SubscriptionEndpoint.cs to use Subscription-related commands/queries
- **FR-011**: System MUST update WebhookEndpoint.cs to use Subscription instead of DriverSubscription
- **FR-012**: System MUST update IDriverPaymentService interface to use Subscription instead of DriverSubscription
- **FR-013**: System MUST remove the DriverSubscription migration file (20250901000000_AddDriverSubscriptionsTable.cs) and create a new migration to add columns (AsaasPaymentId, AsaasPaymentLink, AsaasSubscriptionId, PaidAt) to the existing subscriptions table
- **FR-014**: System MUST ensure no compilation errors exist after the unification

### Key Entities *(include if feature involves data)*

- **Subscription**: The unified subscription entity that will replace both Subscription and DriverSubscription. This is a generic entity serving both drivers and passengers. Key attributes:
  - UserId (long) - Can reference both driver and passenger users
  - PlanId (long)
  - Status (SubscriptionStatus)
  - SubscriptionType (SubscriptionType enum: Driver, Passenger) - *NEW: to differentiate user types*
  - DueDate (DateTime)
  - PaymentToken (string?)
  - AsaasPaymentId (string?) - *from DriverSubscription*
  - AsaasPaymentLink (string?) - *from DriverSubscription*
  - AsaasSubscriptionId (string?) - *from DriverSubscription*
  - PaidAt (DateTime?) - *from DriverSubscription*
  - User (User entity) - Can be either driver or passenger
  - Plan (Plan entity)

- **SubscriptionType**: Enum to differentiate subscription types:
  - Driver (for driver subscriptions)
  - Passenger (for passenger subscriptions)

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Codebase has zero references to DriverSubscription after unification (verified by search)
- **SC-002**: Application compiles successfully without any DriverSubscription-related errors
- **SC-003**: All subscription-related endpoints (create, get, activate, cancel) work correctly with unified Subscription entity
- **SC-004**: Database operations (CRUD) on subscriptions work correctly with the existing subscriptions table
- **SC-005**: A new migration is created and executed to add the missing columns (AsaasPaymentId, AsaasPaymentLink, AsaasSubscriptionId, PaidAt) to the existing subscriptions table

## Assumptions

- The existing `subscriptions` table in the database does NOT have the new columns (AsaasPaymentId, AsaasPaymentLink, AsaasSubscriptionId, PaidAt, SubscriptionType) yet
- A new migration will be required to add these columns to the existing subscriptions table
- The Subscription entity already exists and is partially implemented
- The DriverSubscription migration (20250901000000_AddDriverSubscriptionsTable.cs) was never executed, so no rollback is needed
- All functionality currently implemented for DriverSubscription should be preserved for Subscription
- The project uses Entity Framework Core with PostgreSQL
- The existing Subscription entity database configuration (SubscriptionConfiguration) must be updated to include the new properties
- Existing records in the `subscriptions` table will default to SubscriptionType = Driver (backward compatibility)
