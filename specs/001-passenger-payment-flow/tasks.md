# Tasks: Passenger Travel Payment Flow

**Input**: Design documents from `/specs/001-passenger-payment-flow/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Tests are NOT included (not explicitly requested in feature specification).

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [X] T001 Verify existing project structure matches plan.md (Application/, Domain/, Infrastructure/, Api/)
- [X] T002 [P] Verify existing PrecifyOrderCommand in Application/Features/Orders/Commands/Precify/
- [X] T003 [P] Verify existing CreateOrderCommand in Application/Features/Orders/Commands/Create/
- [X] T004 [P] Verify existing DriverPaymentService in Infrastructure/External/Features/Payments/Services/
- [X] T005 Create directory Application/Features/Payments/Commands/CreatePassengerPayment/
- [X] T006 Create directory Infrastructure/External/Features/Payments/Contracts/

**Checkpoint**: Setup complete - all necessary directories exist

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [X] T007 Add PassengerRide = 3 to PaymentType enum in Domain/Features/Payments/Enums/PaymentType.cs
- [X] T008 [P] Add PaymentId property to Order entity in Domain/Features/Orders/Entities/Order.cs
- [X] T009 [P] Add OrderId property to Payment entity in Domain/Features/Payments/Entities/Payment.cs
- [X] T010 [P] Add AsaasPaymentId property to Payment entity in Domain/Features/Payments/Entities/Payment.cs
- [X] T011 [P] Add PixQrCode property to Payment entity in Domain/Features/Payments/Entities/Payment.cs
- [X] T012 [P] Add PixLink property to Payment entity in Domain/Features/Payments/Entities/Payment.cs
- [X] T013 Create database migration to add PaymentId column to Orders table (Infrastructure/Data/Postgreesql/Migrations/)
- [X] T014 Create database migration to add OrderId, AsaasPaymentId, PixQrCode, PixLink columns to Payments table (Infrastructure/Data/Postgreesql/Migrations/)
- [X] T015 [P] Create IPassengerPaymentService interface in Application/Features/Payments/Contracts/IPassengerPaymentService.cs
- [X] T016 [P] Create PaymentResponse contract in Application/Features/Payments/Contracts/PaymentResponse.cs
- [X] T017 [P] Create PaymentLinkRequest contract in Infrastructure/External/Features/Payments/Contracts/PaymentLinkRequest.cs
- [X] T018 [P] Create PaymentLinkResponse contract in Infrastructure/External/Features/Payments/Contracts/PaymentLinkResponse.cs

**Checkpoint**: Foundation ready - user story implementation can now begin

---

## Phase 3: User Story 1 - Price Travel Order (Priority: P1) 🎯 MVP

**Goal**: Passenger can price a travel order and see estimated cost before committing

**Independent Test**: Can be fully tested by requesting a price estimate with origin/destination and verifying a cost is returned to the passenger

### Implementation for User Story 1

- [X] T019 [US1] Verify existing PrecifyOrderCommand in Application/Features/Orders/Commands/Precify/PrecifyOrderCommand.cs (reuse as-is)
- [X] T020 [US1] Verify existing PrecifyOrderCommandHandler in Application/Features/Orders/Commands/Precify/PrecifyOrderCommandHandler.cs (reuse as-is)
- [X] T021 [US1] Verify existing PrecifyOrderCommandValidator in Application/Features/Orders/Commands/Precify/PrecifyOrderCommandValidator.cs (reuse as-is)
- [X] T022 [US1] Verify OrderResponse contract in Application/Features/Orders/Contracts/OrderResponse.cs (reuse as-is)

**Checkpoint**: At this point, User Story 1 should be fully functional (pricing already exists)

---

## Phase 4: User Story 2 - Create Passenger Wallet (Priority: P2)

**Goal**: Passenger has a digital wallet created in Asaas to enable payment processing

**Independent Test**: Can be fully tested by submitting passenger information and verifying a wallet is created and linked to the passenger account

### Implementation for User Story 2

- [X] T023 [US2] Create CreateOrGetWalletAsync method in Infrastructure/External/Features/Payments/Services/PassengerPaymentService.cs (follow DriverPaymentService.CreateDriverWalletAsync pattern)
- [X] T024 [US2] Add SetAsaasWalletId method call in User entity (already exists: Domain/Features/Users/Entities/User.cs line 187)
- [X] T025 [US2] Implement wallet creation logic: check User.AsaasWalletId → return if exists → POST /v3/accounts → save wallet ID

**Checkpoint**: At this point, User Story 2 should be fully functional - wallets can be created for passengers

---

## Phase 5: User Story 3 - Process Payment for Order (Priority: P1) 🎯 MVP

**Goal**: Passenger makes a payment for the priced travel order using PIX via Asaas

**Independent Test**: Can be fully tested by submitting payment for a priced order and verifying payment is processed successfully with confirmation

### Implementation for User Story 3

- [X] T026 [US3] Create CreatePassengerPaymentCommand record in Application/Features/Payments/Commands/CreatePassengerPayment/CreatePassengerPaymentCommand.cs
- [X] T027 [US3] Create CreatePassengerPaymentCommandHandler in Application/Features/Payments/Commands/CreatePassengerPayment/CreatePassengerPaymentCommandHandler.cs (implement ICommandHandler<CreatePassengerPaymentCommand, PaymentResponse>)
- [X] T028 [US3] Create CreatePassengerPaymentCommandValidator in Application/Features/Payments/Commands/CreatePassengerPayment/CreatePassengerPaymentCommandValidator.cs
- [X] T029 [US3] Implement CreatePaymentAsync method in Infrastructure/External/Features/Payments/Services/PassengerPaymentService.cs (POST /v3/paymentLinks, return paymentId, pixLink, qrCode)
- [X] T030 [US3] Implement CheckPaymentStatusAsync method in Infrastructure/External/Features/Payments/Services/PassengerPaymentService.cs (GET /v3/payments/{id}, check status == "CONFIRMED")
- [X] T031 [US3] Register PassengerPaymentService in DependencyInjection configuration (Application/Shared/Extensions/ or similar)
- [X] T032 [US3] Create endpoint in Api/Endpoints/PaymentsEndpoint.cs for POST /api/payments/passenger/create/

**Checkpoint**: At this point, User Stories 1, 2, and 3 should all work - passengers can price, create wallet, and make payments

---

## Phase 6: User Story 4 - Create Order After Payment (Priority: P1) 🎯 MVP

**Goal**: Travel order is created and confirmed only after successful payment, making the order available for driver matching

**Independent Test**: Can be fully tested by verifying that an order is only created after successful payment and appears in the system for driver matching

### Implementation for User Story 4

- [X] T033 [US4] Modify CreateOrderCommand in Application/Features/Orders/Commands/Create/CreateOrderCommand.cs to add AsaasPaymentId parameter
- [X] T034 [US4] Modify CreateOrderCommandHandler in Application/Features/Orders/Commands/Create/CreateOrderCommandHandler.cs:
  - Validate payment exists and is approved (call PassengerPaymentService.CheckPaymentStatusAsync)
  - Set Order status to ReadyToAccept (not PendingPayment)
  - Link Payment to Order (set Payment.OrderId = order.Id)
  - Update Payment status to Approved
- [X] T035 [US4] Modify CreateOrderCommandValidator in Application/Features/Orders/Commands/Create/CreateOrderCommandValidator.cs to validate AsaasPaymentId is not empty
- [X] T036 [US4] Add webhook handler in Api/Endpoints/WebhookEndpoint.cs for passenger ride payments (POST /api/webhooks/asaas/passenger-ride)
- [X] T037 [US4] In webhook handler: extract externalReference → verify payment status → call CreateOrderCommand with paymentId
- [X] T038 [US4] Update OrderStatus transition validation to allow PendingPayment → ReadyToAccept (Domain/Features/Orders/Entities/Order.cs)

**Checkpoint**: All user stories should now be independently functional - complete flow works: Price → Wallet → Payment → Order

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [X] T039 [P] Update Api/Endpoints/OrdersEndpoint.cs to ensure CreateOrder endpoint requires AsaasPaymentId
- [X] T040 [P] Update Api/Endpoints/PaymentsEndpoint.cs with proper authorization (user can only create payments for themselves)
- [X] T041 Add logging for all payment operations (follow existing _logger patterns)
- [X] T042 Add error handling for Asaas API failures (follow DriverPaymentService patterns)
- [X] T043 [P] Update documentation: Docs/DOCUMENTATION-INDEX.md to include new payment flow
- [X] T044 [P] Create or update README.md with passenger payment flow information
- [X] T045 Run quickstart.md validation to ensure all steps are accurate

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3-6)**: All depend on Foundational phase completion
  - US1 (Phase 3) can start after Foundational - NO dependencies on other stories
  - US2 (Phase 4) can start after Foundational - May integrate with US1 but should be independently testable
  - US3 (Phase 5) can start after Foundational - Depends on US2 (wallet) being complete
  - US4 (Phase 6) can start after Foundational - Depends on US3 (payment) being complete
- **Polish (Phase 7)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2) - No dependencies on other stories
- **User Story 2 (P2)**: Can start after Foundational (Phase 2) - No dependencies on US1
- **User Story 3 (P1)**: Depends on US2 completion (wallet must exist before payment)
- **User Story 4 (P1)**: Depends on US3 completion (payment must be confirmed before order creation)

### Within Each User Story

- Contracts before implementation
- Domain entities before Application handlers
- Infrastructure services before Application handlers
- Handlers before endpoints
- Story complete before moving to next priority

### Parallel Opportunities

- All Setup tasks marked [P] can run in parallel
- All Foundational tasks marked [P] can run in parallel (within Phase 2)
- Once Foundational phase completes:
  - US1 and US2 can run in parallel (no dependencies between them)
  - US3 can start after US2 completes
  - US4 can start after US3 completes
- All Polish tasks marked [P] can run in parallel

---

## Parallel Example: User Story 1 (Pricing - Already Exists)

```bash
# These tasks can run in parallel (all [P] and independent files):
T002 - Verify PrecifyOrderCommand
T003 - Verify CreateOrderCommand  
T004 - Verify DriverPaymentService
T005 - Create CreatePassengerPayment directory
T006 - Create Contracts directory
```

---

## Summary

**Total Tasks**: 45

**By User Story**:
- Phase 1 (Setup): 6 tasks
- Phase 2 (Foundational): 12 tasks
- Phase 3 (US1 - Price Order): 4 tasks (verification of existing)
- Phase 4 (US2 - Create Wallet): 3 tasks
- Phase 5 (US3 - Process Payment): 7 tasks
- Phase 6 (US4 - Create Order): 6 tasks
- Phase 7 (Polish): 7 tasks

**Parallel Opportunities**: 
- 18 tasks marked [P] can run in parallel
- US1 and US2 can run in parallel after Phase 2
- MVP Scope: US1 + US3 + US4 (all P1) = Core payment flow

**Independent Test Criteria**:
- US1: Price order → verify cost returned
- US2: Create wallet → verify wallet ID saved to user
- US3: Process payment → verify PIX link returned
- US4: Payment confirmed → verify order created with ReadyToAccept status

**Suggested MVP**: Implement US1 (exists) + US2 + US3 + US4 for complete payment flow
