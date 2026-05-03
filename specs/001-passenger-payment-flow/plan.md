# Implementation Plan: Passenger Travel Payment Flow

**Branch**: `payments` | **Date**: May 3, 2026 | **Spec**: [spec.md](../spec.md)
**Input**: Feature specification from `/specs/001-passenger-payment-flow/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

Implement the complete passenger travel payment flow integrating with Asaas payment provider. The flow consists of: (1) Pricing the travel order using existing `PrecifyOrderCommand`, (2) Creating/retrieving passenger wallet in Asaas, (3) Processing payment via Asaas, and (4) Creating the order only after successful payment confirmation. The existing `CreateOrderCommand` will be modified to require payment confirmation before order creation.

## Technical Context

**Language/Version**: C# 10 / .NET 10  
**Primary Dependencies**: ASP.NET Core, Entity Framework Core, FluentResults, FluentValidation, RestSharp, Asaas API  
**Storage**: PostgreSQL via Entity Framework Core  
**Testing**: xUnit (assumed based on Tests project structure)  
**Target Platform**: Linux server (backend API)  
**Project Type**: Web service API  
**Performance Goals**: Price estimates < 5s, Payment processing < 30s for 95% transactions  
**Constraints**: Must integrate with Asaas payment provider, Zero orders without payment, Payment failure rate < 5%  
**Scale/Scope**: Single passenger payment flow, Asaas wallet creation for new passengers, PIX payment processing

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**MANDATORY CHECKS** (per Tilt API Constitution v1.0.0):
1. ✅ CQRS Pattern: All handlers use `ICommandHandler<>` or `IQueryHandler<>` (NO MediatR)
2. ✅ Documentation: AI agent files (`.github/copilot-instructions.md`) are synchronized
3. ✅ Templates: This plan follows `.specify/templates/plan-template.md` exactly
4. ✅ Code Examples: Examples use real project patterns from `Application/Features/`
5. ✅ Language: Documentation in Portuguese (pt-BR) unless technical terms

**Validation**: Constitution checks pass - no violations detected.

## Project Structure

### Documentation (this feature)

```text
specs/001-passenger-payment-flow/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
Application/
├── Features/
│   ├── Orders/
│   │   ├── Commands/
│   │   │   ├── Precify/           # EXISTING - Pricing logic (reuse)
│   │   │   └── Create/            # EXISTING - Modify to require payment
│   │   └── Contracts/             # EXISTING - OrderResponse
│   └── Payments/
│       ├── Commands/
│       │   ├── CaptureRidePayment/     # EXISTING - Reference for patterns
│       │   ├── CreateDriverTransfer/   # EXISTING - Reference only
│       │   ├── RequestRidePaymentAuthorization/ # EXISTING - Reference only
│       │   └── CreatePassengerPayment/ # NEW - Process passenger payment
│       └── Contracts/                   # NEW - Payment response contracts

Domain/
├── Features/
│   ├── Orders/
│   │   ├── Entities/Order.cs      # EXISTING - Has Status (PendingPayment)
│   │   └── Enums/OrderStatus.cs  # EXISTING - PendingPayment = 1
│   └── Payments/
│       ├── Entities/Payment.cs    # EXISTING - Payment entity
│       └── Enums/PaymentStatus.cs # EXISTING - Pending, Approved, Cancelled

Infrastructure/
├── External/
│   └── Features/
│       └── Payments/
│           ├── Services/
│           │   ├── DriverPaymentService.cs  # EXISTING - Reference for wallet/payment patterns
│           │   └── PassengerPaymentService.cs # NEW - Passenger payment service
│           └── Contracts/                   # NEW - Add passenger payment contracts
└── Data/
    └── Postgreesql/  # EXISTING - Database context
```

**Structure Decision**: Follow existing project structure with CQRS pattern. Reuse existing `PrecifyOrderCommand` for pricing. Create new `CreatePassengerPaymentCommand` following the `DriverPaymentService` patterns for Asaas integration. Modify `CreateOrderCommand` to validate payment before creating order.

## Complexity Tracking

> **No violations - Constitution Check passed**

No complexity exceptions needed. Following standard project patterns.

---

## Phase 0: Research & Analysis

### Research Findings

**Decision**: Use existing `DriverPaymentService` patterns for Asaas integration
- **Rationale**: Project already has working Asaas integration for driver subscriptions
- **Pattern**: Use RestSharp for HTTP calls, follow same header/auth pattern
- **Wallet Creation**: Reuse pattern from `CreateDriverWalletAsync` method

**Payment Flow Decision**: PIX payment for passengers
- **Rationale**: Consistent with driver subscription payments (PIX)
- **Implementation**: Create payment link via Asaas API, return PIX code to frontend
- **Confirmation**: Webhook from Asaas → update payment status → create order

**Order Creation Decision**: Modify existing `CreateOrderCommand`
- **Rationale**: Order already has `PendingPayment` status
- **Change**: Add payment validation before saving order
- **Flow**: Payment approved → CreateOrderCommand → Order status = ReadyToAccept

### Resolved Unknowns

1. **Wallet Creation**: Use `CreateDriverWalletAsync` pattern from `DriverPaymentService`
2. **Payment Processing**: Create payment link via POST `/v3/paymentLinks` (same as subscriptions)
3. **Order Creation**: Modify `CreateOrderCommand` to require `paymentId` parameter
4. **Payment Verification**: Use webhook or polling (webhook preferred based on existing pattern)

---

## Phase 1: Design & Contracts

### Data Model

**Order Entity** (Existing - `Domain/Features/Orders/Entities/Order.cs`)
- Status: `PendingPayment` → `ReadyToAccept` (after payment)
- New property: `PaymentId` (links to Payment entity)

**Payment Entity** (Existing - `Domain/Features/Payments/Entities/Payment.cs`)
- Type: Add `PassengerRide` to `PaymentType` enum
- Status: `Pending` → `Approved` (on successful payment)
- New property: `OrderId` (links payment to order)

**PaymentType Enum** (Existing - modify)
- Add: `PassengerRide = 3`

### Contracts

**CreatePassengerPaymentCommand** (New)
```csharp
public record CreatePassengerPaymentCommand(
    long UserId,
    decimal Amount,
    string Description,
    string ExternalReference  // Will be Order ID
) : IRequest<Result<PaymentResponse>>;
```

**PaymentResponse** (New)
```csharp
public class PaymentResponse
{
    public string PaymentId { get; set; }      // Asaas payment ID
    public string PaymentLink { get; set; }     // PIX link for frontend
    public string QrCode { get; set; }         // PIX QR code
    public decimal Amount { get; set; }
    public string Status { get; set; }          // PENDING, CONFIRMED, etc.
}
```

### Quickstart Guide

**Flow for Frontend**:
1. Passenger requests price: `POST /api/orders/precify` → returns price estimate
2. Passenger initiates payment: `POST /api/payments/passenger/create` → returns PIX code/link
3. Frontend displays PIX QR code to passenger
4. Asaas sends webhook on payment confirmation
5. Webhook handler creates order: calls modified `CreateOrderCommand` with paymentId
6. Order status = `ReadyToAccept` → available for driver matching

---

## Phase 2: Implementation Tasks

See `tasks.md` (generated by `/speckit.tasks` command)

---

## Agent Context Update

**Action Required**: Update `.github/copilot-instructions.md` to reference this plan:

```markdown
<!-- SPECKIT START -->
Feature implementation plan: specs/001-passenger-payment-flow/plan.md
<!-- SPECKIT END -->
```
