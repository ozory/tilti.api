# Tasks: Cancel Order Flow

**Input**: Design documents from `/specs/003-order-cancel-flow/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: The examples below include test tasks. Tests are OPTIONAL - only include them if explicitly requested in the feature specification.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Single project**: `src/`, `tests/` at repository root
- **Web app**: `backend/src/`, `frontend/src/`
- **Mobile**: `api/src/`, `ios/src/` or `android/src/`
- Paths shown below assume single project - adjust based on plan.md structure

<!-- 
  ============================================================================
  IMPORTANT: The tasks below are SAMPLE TASKS for illustration purposes only.
  
  The /speckit.tasks command MUST replace these with actual tasks based on:
  - User stories from spec.md (with their priorities P1, P2, P3...)
  - Feature requirements from plan.md
  - Entities from data-model.md
  - Endpoints from contracts/
  
  Tasks MUST be organized by user story so each story can be:
  - Implemented independently
  - Tested independently
  - Delivered as an MVP increment
  
  DO NOT keep these sample tasks in the generated tasks.md file.
  ============================================================================
-->

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [x] T001 Verify project structure matches implementation plan (Api/, Application/, Domain/, Infrastructure/, Tests/, Resources/, Docs/, specs/)
- [ ] T002 Initialize .NET project with ASP.NET Core dependencies
- [ ] T003 [P] Configure linting and formatting tools

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

Examples of foundational tasks (adjust based on your project):

- [ ] T004 Verify database schema supports cancellation fields (already exists per data-model.md)
- [ ] T005 [P] Ensure RabbitMQ connectivity and configuration
- [ ] T006 [P] Setup API routing and middleware structure
- [ ] T007 [P] Verify Order entity has required cancellation properties (CancelDescription, CancelRasons, CancelledBy, CancelationTime)
- [ ] T008 Configure error handling and logging infrastructure
- [ ] T009 Setup environment configuration management

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Cancel Order Before Driver Acceptance (Priority: P1) 🎯 MVP

**Goal**: Implement core order cancellation functionality that allows users to cancel orders before driver acceptance, with proper validation and status updates.

**Independent Test**: Can be tested by creating an order with status "Pending" and no driver assigned, then issuing a cancel request. The system should return a successful cancellation, set order status to "Canceled", and emit a "OrderCanceledPaymentRefund" event.

### Tests for User Story 1 (OPTIONAL - only if tests requested) ⚠️

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [ ] T010 [P] [US1] Contract test for POST /api/orders/{orderId}/cancel endpoint in tests/contract/test_cancel_order.py
- [ ] T011 [P] [US1] Integration test for cancel order before driver acceptance in tests/integration/test_cancel_order_pending.py

### Implementation for User Story 1

- [x] T012 [P] [US1] Create CancelOrderCommand in Application/Features/Orders/Commands/Cancel/CancelOrderCommand.cs
- [x] T013 [P] [US1] Create CancelOrderCommandValidator in Application/Features/Orders/Commands/Cancel/CancelOrderCommandValidator.cs
- [x] T014 [US1] Create CancelOrderCommandHandler in Application/Features/Orders/Commands/Cancel/CancelOrderCommandHandler.cs (depends on T012, T013)
- [x] T015 [US1] Implement cancellation validation logic in handler (check order status != InTransit)
- [x] T016 [US1] Update Order entity status to Canceled and set CancelledBy = "User"
- [x] T017 [US1] Set CancelDescription and CancelRasons from command
- [x] T018 [US1] Set CancelationTime to current UTC time
- [x] T019 [US1] Publish OrderCanceledPaymentRefund message to RabbitMQ when user cancels
- [x] T020 [US1] Create CancelOrderEndpoint in Api/Endpoints/OrdersEndpoint.cs (or create new CancelOrderEndpoint.cs)
- [x] T021 [US1] Add validation and error handling for not found orders
- [x] T022 [US1] Add logging for cancel order operations

**Checkpoint**: At this point, User Story 1 should be fully functional and testable independently

---

## Phase 4: User Story 2 - Prevent Cancellation During Active Ride (Priority: P1)

**Goal**: Implement validation to prevent cancellation of orders that are already in progress (InTransit status).

**Independent Test**: Can be tested by creating an order with status "InTransit" and attempting to cancel. The system should reject the cancellation with an appropriate error.

### Tests for User Story 2 (OPTIONAL - only if tests requested) ⚠️

- [ ] T023 [P] [US2] Contract test for POST /api/orders/{orderId}/cancel endpoint returning 400 for InTransit orders
- [ ] T024 [P] [US2] Integration test for cancel order prevention during active ride in tests/integration/test_cancel_order_inprogress.py

### Implementation for User Story 2

- [x] T025 [P] [US2] Enhance CancelOrderCommandHandler to check for InTransit status and return appropriate error
- [x] T026 [US2] Add specific error message: "Não é possível cancelar um pedido em andamento" (Cannot cancel an order in progress)
- [x] T027 [US2] Ensure handler returns Result.Fail with appropriate error for InTransit orders
- [x] T028 [US2] Add logging for blocked cancellation attempts

**Checkpoint**: At this point, User Stories 1 AND 2 should both work independently

---

## Phase 5: User Story 3 - Identify Who Initiated Cancellation (Priority: P2)

**Goal**: Track who initiated the cancellation (User or Driver) and persist this information in the Order entity.

**Independent Test**: Can be tested by issuing a cancel request and checking the "CancelledBy" field in the order record.

### Tests for User Story 3 (OPTIONAL - only if tests requested) ⚠️

- [ ] T029 [P] [US3] Contract test for verifying CancelledBy field is set correctly
- [ ] T030 [P] [US3] Integration test for tracking who initiated cancellation in tests/integration/test_cancelled_by_tracking.py

### Implementation for User Story 3

- [x] T031 [P] [US3] Ensure CancelOrderCommandHandler sets CancelledBy = "User" for user-initiated cancellations
- [x] T032 [US3] Verify CancelledBy property is persisted to database
- [x] T033 [US3] Prepare infrastructure for driver-initiated cancellations (will be implemented when driver cancellation feature is added)
- [x] T034 [US3] Add logging to track who initiated each cancellation

**Checkpoint**: All user stories should now be independently functional

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [x] T035 [P] Documentation updates in docs/
- [x] T036 Code cleanup and refactoring
- [x] T037 [P] Additional unit tests (if requested) in tests/unit/
- [ ] T038 Run quickstart.md validation
- [ ] T039 Verify RabbitMQ message format matches contract specification
- [x] T040 Ensure proper error handling for all edge cases (order not found, already cancelled, etc.)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3+)**: All depend on Foundational phase completion
  - User stories can then proceed in parallel (if staffed)
  - Or sequentially in priority order (P1 → P2 → P3)
- **Polish (Final Phase)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2) - No dependencies on other stories
- **User Story 2 (P2)**: Can start after Foundational (Phase 2) - May integrate with US1 but should be independently testable
- **User Story 3 (P3)**: Can start after Foundational (Phase 2) - May integrate with US1/US2 but should be independently testable

### Within Each User Story

- Tests (if included) MUST be written and FAIL before implementation
- Models before services
- Services before endpoints
- Core implementation before integration
- Story complete before moving to next priority

### Parallel Opportunities

- All Setup tasks marked [P] can run in parallel
- All Foundational tasks marked [P] can run in parallel (within Phase 2)
- Once Foundational phase completes, all user stories can start in parallel (if team capacity allows)
- All tests for a user story marked [P] can run in parallel
- Models within a story marked [P] can run in parallel
- Different user stories can be worked on in parallel by different team members

---

## Parallel Example: User Story 1

```bash
# These tasks can run in parallel for User Story 1:
# T012 [P] [US1] Create CancelOrderCommand
# T013 [P] [US1] Create CancelOrderCommandValidator
# Once T012 and T013 complete, T014 can start
```