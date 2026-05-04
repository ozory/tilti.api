# Quickstart: Unify Subscriptions

**Date**: 2026-05-03  
**Feature**: 002-unify-subscriptions  
**Purpose**: Quick reference guide for developers implementing the subscription unification.

## Overview

This feature unifies `Subscription` and `DriverSubscription` entities into a single `Subscription` entity that supports both drivers and passengers.

## Key Changes Summary

| Change | Description |
|--------|-------------|
| **New Enum** | `SubscriptionType` (Driver, Passenger) added to `Domain/Subscriptions/Enums/` |
| **Updated Entity** | `Subscription.cs` now includes `AsaasPaymentId`, `AsaasPaymentLink`, `AsaasSubscriptionId`, `PaidAt`, and `SubscriptionType` |
| **Updated Status** | `SubscriptionStatus` enum now includes `PaymentFailed` |
| **Database** | New migration adds columns to existing `tilt.subscriptions` table |
| **Removed** | `DriverSubscription.cs`, `DriverSubscriptionConfiguration.cs`, `IDriverSubscriptionRepository.cs` |
| **Replaced** | All references to `DriverSubscription` → `Subscription` across all layers |

## Implementation Steps (Ordered)

### 1. Domain Layer (First)

```bash
# 1.1 Create SubscriptionType enum
# Path: Domain/Subscriptions/Enums/SubscriptionType.cs
dotnet new class -n SubscriptionType -o Domain/Features/Subscriptions/Enums

# 1.2 Update SubscriptionStatus enum (add PaymentFailed)
# Path: Domain/Subscriptions/Enums/SubscriptionStatus.cs

# 1.3 Update Subscription entity
# Path: Domain/Features/Subscriptions/Entities/Subscription.cs
# - Add: SubscriptionType, AsaasPaymentId, AsaasPaymentLink, AsaasSubscriptionId, PaidAt
# - Update: Create() method to accept SubscriptionType

# 1.4 Remove DriverSubscription entity
rm Domain/Features/Subscriptions/Entities/DriverSubscription.cs
```

### 2. Application Layer

```bash
# 2.1 Update all Subscription Commands
# Paths: Application/Features/Subscriptions/Commands/*/  
# - Replace DriverSubscription → Subscription
# - Update handlers to use ISubscriptionRepository

# 2.2 Update all Subscription Queries
# Paths: Application/Features/Subscriptions/Queries/*/
# - Replace DriverSubscription → Subscription
# - Update handlers to use ISubscriptionRepository

# 2.3 Update Contracts (DTOs)
# Path: Application/Features/Subscriptions/Contracts/
# - Add SubscriptionType to response DTOs
```

### 3. Infrastructure Layer

```bash
# 3.1 Update SubscriptionConfiguration
# Path: Infrastructure/Data/Postgreesql/Features/Subscriptions/Configurations/SubscriptionConfiguration.cs
# - Add new properties mapping
# - Add unique index for (UserId, SubscriptionType, Status)

# 3.2 Update SubscriptionRepository
# Path: Infrastructure/Data/Postgreesql/Features/Subscriptions/Repository/SubscriptionRepository.cs
# - Replace IDriverSubscriptionRepository → ISubscriptionRepository
# - Update method signatures

# 3.3 Remove DriverSubscriptionConfiguration
rm Infrastructure/Data/Postgreesql/Features/Subscriptions/Configurations/DriverSubscriptionConfiguration.cs

# 3.4 Remove DriverSubscriptionRepository
rm Infrastructure/Data/Postgreesql/Features/Subscriptions/Repository/DriverSubscriptionRepository.cs

# 3.5 Update TILTContext
# Path: Infrastructure/Data/Postgreesql/TILTContext.cs
# - Remove: DbSet<DriverSubscription>
# - Ensure DbSet<Subscription> is properly configured

# 3.6 Update DependencyInjection
# Path: Infrastructure/Shared/Configurations/DependencyInjection.cs
# - Replace IDriverSubscriptionRepository → ISubscriptionRepository
# - Replace DriverSubscriptionRepository → SubscriptionRepository
```

### 4. Create Migration

```bash
# Navigate to Infrastructure project
cd Infrastructure

# Create migration (from Infrastructure project directory)
dotnet ef migrations add AddSubscriptionUnificationColumns \
    --startup-project ../Api \
    --context TILTContext

# Review generated migration file
# Ensure it adds columns to existing subscriptions table (not create new table)

# Apply migration
dotnet ef database update \
    --startup-project ../Api \
    --context TILTContext
```

### 5. API Layer

```bash
# 5.1 Update SubscriptionEndpoint
# Path: Api/Endpoints/SubscriptionEndpoint.cs
# - Replace DriverSubscription commands/queries with Subscription ones
# - Update route names if needed

# 5.2 Update WebhookEndpoint
# Path: Api/Endpoints/WebhookEndpoint.cs
# - Replace DriverSubscription → Subscription in webhook handlers

# 5.3 Update IDriverPaymentService
# Path: Application/Shared/Abstractions/IDriverPaymentService.cs
# - Replace DriverSubscription parameter → Subscription
```

## Verification Checklist

- [ ] `grep -r "DriverSubscription" .` returns no results (except maybe in specs/docs)
- [ ] Application compiles: `dotnet build` succeeds
- [ ] Database migration applied: `dotnet ef database update` succeeds
- [ ] New columns exist in database: check `tilt.subscriptions` table
- [ ] SubscriptionType enum created with Driver=1, Passenger=2
- [ ] SubscriptionStatus enum has PaymentFailed value
- [ ] All endpoints return correct Subscription data (test with curl/Postman)

## Common Pitfalls

1. **Forgetting to update DI registrations**: Ensure `DependencyInjection.cs` registers correct types
2. **Migration creates new table**: Ensure migration adds columns to existing `subscriptions` table
3. **Unique constraint too restrictive**: Only apply uniqueness to active/pending subscriptions
4. **Existing data not handled**: Ensure default value for SubscriptionType is "Driver"

## Rollback Plan (if needed)

```bash
# 1. Revert migration
dotnet ef database update <previous-migration-name> \
    --startup-project ../Api \
    --context TILTContext

# 2. Revert code changes (use git)
git revert <commit-hash>

# 3. Restore DriverSubscription entity from git history if needed
git checkout <previous-commit> -- Domain/Features/Subscriptions/Entities/DriverSubscription.cs
```
