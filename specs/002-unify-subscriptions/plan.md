# Implementation Plan: [FEATURE]

**Branch**: `[###-feature-name]` | **Date**: [DATE] | **Spec**: [link]
**Input**: Feature specification from `/specs/[###-feature-name]/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

Unificar as entidades `Subscription` e `DriverSubscription` em uma única entidade `Subscription` genérica que suporta tanto motoristas quanto passageiros. A entidade unificada deve incluir todas as propriedades de `DriverSubscription` (`AsaasPaymentId`, `AsaasPaymentLink`, `AsaasSubscriptionId`, `PaidAt`) e um novo enum `SubscriptionType` (Driver, Passenger) para diferenciar os tipos. A tabela existente `subscriptions` no banco de dados será atualizada via nova migration para incluir as novas colunas. Todas as referências a `DriverSubscription` no código serão removidas ou substituídas por `Subscription`.

## Technical Context

**Language/Version**: C# 12, .NET 8  
**Primary Dependencies**: Entity Framework Core 8 (Npgsql for PostgreSQL), FluentResults, FluentValidation, Asaas API Client (external payment service)  
**Storage**: PostgreSQL (existing `subscriptions` table in `tilt` schema)  
**Testing**: xUnit (project has `Tests/` folder with Application, Domain, Infrastructure test projects)  
**Target Platform**: Linux server (backend REST API)  
**Project Type**: Web API (ASP.NET Core)  
**Performance Goals**: p95 latency <200ms for subscription queries, support 1000 concurrent users  
**Constraints**:  
- Must preserve existing data in `subscriptions` table (default new `SubscriptionType` column to "Driver" for existing records)  
- Must not use MediatR (project uses custom `ICommandHandler<T>`/`IQueryHandler<T>` interfaces)  
- Must follow CQRS pattern with FluentResults for error handling  
**Scale/Scope**: Existing `subscriptions` table with unknown record count; unify two domain entities into one; update ~50+ code references from `DriverSubscription` to `Subscription`

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**MANDATORY CHECKS** (per Tilt API Constitution v1.0.0):
1. ✅ CQRS Pattern: All handlers use `ICommandHandler<>` or `IQueryHandler<>` (NO MediatR)
2. ✅ Documentation: AI agent files (`.agent.md`, `.cursor/rules.md`) are synchronized
3. ✅ Templates: This plan follows `.specify/templates/plan-template.md` exactly
4. ✅ Code Examples: Any examples use real project patterns from `Application/Features/`
5. ✅ Language: Documentation in Portuguese (pt-BR) unless technical terms

**Validation**: Run `/speckit.constitution` to verify compliance

## Project Structure

### Documentation (this feature)

```text
specs/002-unify-subscriptions/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root - relevant paths for this feature)

```text
Domain/
├── Features/Subscriptions/
│   ├── Entities/
│   │   ├── Subscription.cs          # Unified entity (modify)
│   │   └── DriverSubscription.cs   # Remove after migration
│   ├── Enums/
│   │   └── SubscriptionType.cs     # NEW: Add enum (Driver, Passenger)
│   └── Repository/
│       └── ISubscriptionRepository.cs  # Update (was IDriverSubscriptionRepository)

Application/
├── Features/Subscriptions/
│   ├── Commands/                     # Update all commands to use Subscription
│   │   ├── CreateSubscription/
│   │   ├── ActivateSubscription/
│   │   └── CancelSubscription/
│   ├── Queries/                      # Update all queries to use Subscription
│   └── Contracts/                    # Update response DTOs

Infrastructure/
├── Data/Postgreesql/
│   ├── Features/Subscriptions/
│   │   ├── Configurations/
│   │   │   ├── SubscriptionConfiguration.cs      # Update for new properties
│   │   │   └── DriverSubscriptionConfiguration.cs # Remove
│   │   └── Repository/
│   │       └── SubscriptionRepository.cs        # Update (was DriverSubscriptionRepository)
│   ├── Migrations/                   # Add new migration for columns
│   └── TILTContext.cs                # Remove DbSet<DriverSubscription>
└── Shared/Configurations/
    └── DependencyInjection.cs         # Update DI registrations

Api/
├── Endpoints/
│   ├── SubscriptionEndpoint.cs       # Update to use Subscription commands/queries
│   └── WebhookEndpoint.cs            # Update to use Subscription instead of DriverSubscription
└── Program.cs                        # (if DI changes needed)
```

**Structure Decision**: Single .NET solution with layered architecture (Domain, Application, Infrastructure, Api). This feature modifies existing Subscription-related code across all layers to unify the model.

## Phase 0: Research ✅ (Completed)

**Output**: `research.md`

Key decisions made:
1. **SubscriptionType Enum**: Added to differentiate Driver/Passenger subscriptions
2. **Default Value**: Existing records default to "Driver" for backward compatibility
3. **Uniqueness**: One active subscription per user per SubscriptionType
4. **Payment Failures**: Mark as PaymentFailed + notify user/admin
5. **Migration Strategy**: Add columns to existing `subscriptions` table (no new table)
6. **Code Changes**: Systematic replacement of `DriverSubscription` with `Subscription`

All NEEDS CLARIFICATION resolved.

---

## Phase 1: Design & Contracts ✅ (Completed)

**Outputs**:
- `data-model.md`: Unified Subscription entity with all properties and relationships
- `contracts/subscription-api.md`: Updated API contracts for Commands, Queries, and Endpoints
- `quickstart.md`: Developer guide with step-by-step implementation instructions
- Updated agent context: `.github/copilot-instructions.md` points to this plan

### Key Design Decisions:

| Decision | Description |
|----------|-------------|
| **Entity** | Single `Subscription` entity with `SubscriptionType` enum |
| **Database** | Existing `tilt.subscriptions` table with new columns added via migration |
| **Enums** | `SubscriptionType` (Driver=1, Passenger=2), `SubscriptionStatus` (+PaymentFailed) |
| **Uniqueness** | Unique index on (user_id, subscription_type) for active subscriptions |
| **API** | Unified endpoints: POST `/subscriptions`, GET `/subscriptions/{id}`, etc. |

---

## Phase 2: Implementation Tasks

**Output**: `tasks.md` (generated by `/speckit.tasks` command)

### Task Summary (to be detailed in tasks.md):

**Domain Layer (Priority: P0)**:
- TASK-001: Create `SubscriptionType` enum in `Domain/Subscriptions/Enums/`
- TASK-002: Update `SubscriptionStatus` enum to add `PaymentFailed`
- TASK-003: Update `Subscription.cs` entity with new properties and methods
- TASK-004: Remove `DriverSubscription.cs` entity

**Application Layer (Priority: P1)**:
- TASK-005: Update all Subscription Commands to use `Subscription` entity
- TASK-006: Update all Subscription Queries to use `Subscription` entity
- TASK-007: Update Contracts (DTOs) to include new properties
- TASK-008: Remove `IDriverSubscriptionRepository` interface

**Infrastructure Layer (Priority: P2)**:
- TASK-009: Update `SubscriptionConfiguration.cs` for new properties and indexes
- TASK-010: Update `SubscriptionRepository.cs` to implement `ISubscriptionRepository`
- TASK-011: Remove `DriverSubscriptionConfiguration.cs`
- TASK-012: Remove `DriverSubscriptionRepository.cs`
- TASK-013: Update `TILTContext.cs` to remove `DbSet<DriverSubscription>`
- TASK-014: Create migration to add columns to `subscriptions` table
- TASK-015: Update `DependencyInjection.cs` with correct registrations

**API Layer (Priority: P3)**:
- TASK-016: Update `SubscriptionEndpoint.cs` to use unified Subscription commands/queries
- TASK-017: Update `WebhookEndpoint.cs` to use `Subscription` instead of `DriverSubscription`
- TASK-018: Update `IDriverPaymentService.cs` to use `Subscription` parameter

**Testing & Validation (Priority: P4)**:
- TASK-019: Verify no compilation errors (`dotnet build`)
- TASK-020: Verify no references to `DriverSubscription` in codebase
- TASK-021: Test migration applies successfully (`dotnet database update`)
- TASK-022: Test all subscription endpoints work correctly

---

## Constitution Check (Post-Design)

*Re-check after Phase 1 design is complete.*

**MANDATORY CHECKS** (per Tilt API Constitution v1.0.0):
1. ✅ **CQRS Pattern**: All handlers use `ICommandHandler<>` or `IQueryHandler<>` (NO MediatR)
2. ✅ **Documentation**: AI agent files synchronized, plan follows template
3. ✅ **Templates**: `plan.md` follows `.specify/templates/plan-template.md`
4. ✅ **Code Examples**: Contracts use real project patterns from `Application/Features/`
5. ✅ **Language**: Documentation in Portuguese (pt-BR) unless technical terms
6. ✅ **No MediatR**: All references use project's `ICommandHandler<>`/`IQueryHandler<>`

**Validation**: All checks PASSED.

---

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| (None - all checks passed) | N/A | N/A |
