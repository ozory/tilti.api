# Research: Unify Subscriptions

**Date**: 2026-05-03  
**Feature**: 002-unify-subscriptions  
**Purpose**: Document technical decisions and research findings for unifying Subscription and DriverSubscription entities.

## Decision 1: SubscriptionType Enum

**Decision**: Add `SubscriptionType` enum (Driver, Passenger) to the unified Subscription entity.

**Rationale**:
- The unified Subscription entity must serve both drivers and passengers
- Enum provides type safety and clear differentiation
- Follows existing pattern in the project (e.g., `SubscriptionStatus` enum)
- Allows for easy filtering and business logic based on subscription type

**Alternatives considered**:
- **Boolean `IsDriverSubscription`**: Less extensible if more types are needed later
- **Convention based on Plan type**: Indirect and could lead to inconsistencies
- **Separate tables/entities**: Defeats the purpose of unification

---

## Decision 2: Default Value for Existing Records

**Decision**: Default `SubscriptionType` to "Driver" for existing records in the `subscriptions` table.

**Rationale**:
- The original `DriverSubscription` entity was designed for drivers
- The existing `subscriptions` table was likely created for driver subscriptions
- Maintains backward compatibility
- The new migration will set a default value for the column

**Alternatives considered**:
- **Default to "Passenger"**: Would incorrectly categorize existing driver subscriptions
- **Allow NULL**: Requires null-handling in code and queries
- **Delete existing records**: Not acceptable (data loss)

---

## Decision 3: Uniqueness Constraint

**Decision**: Enforce uniqueness: only one active subscription per user per `SubscriptionType`.

**Rationale**:
- Prevents duplicate active subscriptions of the same type for a user
- Avoids double billing issues
- Standard practice for subscription systems
- Can be enforced via database unique index on (`user_id`, `subscription_type`, `status`) where status is active

**Implementation approach**:
- Add unique index in EF Core configuration (`SubscriptionConfiguration`)
- Check in domain logic before creating new subscription
- Return error via `FluentResults` if duplicate detected

---

## Decision 4: Payment Failure Handling

**Decision**: Mark subscription as "PaymentFailed" and notify user/admin for manual retry.

**Rationale**:
- Safer than automatic retries (avoids potential duplicate charges)
- Follows standard practice in payment systems
- Allows for manual intervention and investigation
- Notification enables proactive customer service

**Implementation approach**:
- Add new status to `SubscriptionStatus` enum: `PaymentFailed`
- Update webhook handler to set this status on payment failure
- Implement notification service (email/notification) - may be out of scope for this feature

---

## Decision 5: Migration Strategy

**Decision**: Create a new migration to add columns to existing `subscriptions` table (not create a new table).

**Rationale**:
- The `subscriptions` table already exists in the database
- `DriverSubscription` migration (`20250901000000_AddDriverSubscriptionsTable.cs`) was never executed
- Adding columns is less risky than table renaming or creation
- Preserves existing data

**Columns to add**:
1. `subscription_type` (varchar(50), default: 'Driver')
2. `asaas_payment_id` (varchar(255), nullable)
3. `asaas_payment_link` (text, nullable)
4. `asaas_subscription_id` (varchar(255), nullable)
5. `paid_at` (timestamp with time zone, nullable)

**Steps**:
1. Update `SubscriptionConfiguration` to include new properties
2. Run `dotnet ef migrations add AddSubscriptionUnificationColumns` in `Infrastructure` project
3. Run `dotnet database update` to apply migration

---

## Decision 6: Removing DriverSubscription References

**Decision**: Systematically replace all `DriverSubscription` references with `Subscription` across the codebase.

**Rationale**:
- Complete unification requires no references to the old entity
- Prevents confusion and maintenance issues
- Ensures compile-time safety (old code won't compile)

**Scope of changes**:
- Domain layer: Remove `DriverSubscription.cs`, update `Subscription.cs`
- Application layer: Update all Commands, Queries, Handlers
- Infrastructure layer: Update Repository, Configuration, TILTContext
- API layer: Update Endpoints
- Shared/Abstractions: Update interfaces like `IDriverPaymentService`

---

## Technical Context Summary

| Aspect | Decision |
|--------|-----------|
| **Entity unification** | Single `Subscription` entity with `SubscriptionType` enum |
| **Existing data** | Default `SubscriptionType` to "Driver" |
| **Uniqueness** | One active subscription per user per type |
| **Payment failures** | Mark as PaymentFailed + notify |
| **Database changes** | New migration to add columns to `subscriptions` table |
| **Code changes** | Systematic replacement of `DriverSubscription` with `Subscription` |
