# Tasks: Refund Penalty Consumer

**Input**: Design documents from `/specs/004-refund-penalty-consumer/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: The examples below include test tasks. Tests are OPTIONAL - only include them if explicitly requested in the feature specification.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3, US4)
- Include exact file paths in descriptions

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Prepare RabbitMQ configuration and consumer registration for the new refund penalty flow

**⚠️ CRITICAL**: No user story work should begin until this phase is complete

- [X] T001 [P] Add `Infrastructure:OrderCanceledPaymentRefundMessages` settings to `Api/appsettings.Development.json`
- [X] T002 [P] Add `Infrastructure:OrderCanceledPaymentRefundMessages` settings to `Api/appsettings.json`
- [X] T003 [P] Register `OrderCanceledPaymentRefundConsumer` in `Application/Shared/Configurations/DependencyInjection.cs`
- [X] T004 [P] Create `Application/Features/Orders/Services/RefundPenaltyCalculationService.cs` skeleton
- [X] T005 [P] Create `Domain/Features/Orders/ValueObjects/PenaltyCalculationResult.cs`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Build the core service and value objects that all user stories depend on

**⚠️ CRITICAL**: No user story work should begin until this phase is complete

- [X] T006 [P] Review and reuse the RabbitMQ consumer pattern from `Application/Features/Orders/Consumers/OrderFinishedConsumer.cs`
- [X] T007 [P] Ensure order acceptance flow captures driver's acceptance geolocation and persists it to the order record
- [X] T008 Implement `RefundPenaltyCalculationService` logic to compute refund penalty and enforce the maximum 80% penalty rule in `Application/Features/Orders/Services/RefundPenaltyCalculationService.cs`
- [X] T009 Implement `PenaltyCalculationResult` value object rules in `Domain/Features/Orders/ValueObjects/PenaltyCalculationResult.cs`
- [X] T010 Create `Tests/Application/Features/Orders/Services/RefundPenaltyCalculationServiceTests.cs` with core penalty calculation unit tests
- [X] T011 Create `Tests/Application/Features/Orders/Consumers/OrderCanceledPaymentRefundConsumerTests.cs` skeleton for consumer tests

**Checkpoint**: Foundation ready - user story implementation can now begin

---

## Phase 3: User Story 1 - Receive Order Canceled Message (Priority: P1)

**Goal**: Implement the RabbitMQ consumer that receives and validates `OrderCanceledPaymentRefund` messages

**Independent Test**: Send an `OrderCanceledPaymentRefund` message to the queue and verify the consumer receives it, validates the order, and invokes penalty calculation.

- [X] T012 [US1] Create `Application/Features/Orders/Consumers/OrderCanceledPaymentRefundConsumer.cs` as a `BackgroundService`
- [X] T013 [US1] Implement message consumption and deserialization using `IMessageRepository` in `Application/Features/Orders/Consumers/OrderCanceledPaymentRefundConsumer.cs`
- [X] T014 [US1] Validate the order exists and has a canceled status using the order repository in `Application/Features/Orders/Consumers/OrderCanceledPaymentRefundConsumer.cs`
- [X] T015 [US1] Invoke `RefundPenaltyCalculationService` from `OrderCanceledPaymentRefundConsumer.cs` when the message is valid
- [X] T016 [US1] Implement error handling for invalid or malformed messages in `Application/Features/Orders/Consumers/OrderCanceledPaymentRefundConsumer.cs`
- [X] T017 [US1] Add logging for message reception, validation success, and rejection in `Application/Features/Orders/Consumers/OrderCanceledPaymentRefundConsumer.cs`

**Checkpoint**: User Story 1 should be independently testable after these tasks

---

## Phase 4: User Story 2 - Calculate Penalty Based on Distance (Priority: P1)

**Goal**: Implement distance-based penalty calculation and ensure refund amount never falls below 20%

**Independent Test**: Validate penalty output for sample distance ratios and confirm maximum 80% penalty.

- [X] T018 [US2] Implement distance ratio-based penalty mapping in `Application/Features/Orders/Services/RefundPenaltyCalculationService.cs`
- [X] T019 [US2] Add logic to enforce minimum refund of 20% in `Application/Features/Orders/Services/RefundPenaltyCalculationService.cs`
- [X] T020 [US2] Add sample calculation unit tests for 0%, 50%, and 90% distance ratios in `Tests/Application/Features/Orders/Services/RefundPenaltyCalculationServiceTests.cs`
- [X] T021 [US2] Update `PenaltyCalculationResult` to include penalty percentage, penalty amount, and refund amount in `Domain/Features/Orders/ValueObjects/PenaltyCalculationResult.cs`
- [X] T022 [US2] Log computed penalty details for each processed event in `Application/Features/Orders/Consumers/OrderCanceledPaymentRefundConsumer.cs`

**Checkpoint**: User Story 2 should be independently testable after these tasks

---

## Phase 5: User Story 3 - Reject Orders Without Driver Acceptance (Priority: P2)

**Goal**: Ensure orders canceled before driver acceptance do not receive a penalty

**Independent Test**: Send a cancellation event for an order without a `DriverId` and verify the service returns 0% penalty.

- [X] T023 [US3] Add driver acceptance validation in `OrderCanceledPaymentRefundConsumer.cs` or `RefundPenaltyCalculationService.cs`
- [X] T024 [US3] Ensure `RefundPenaltyCalculationService` returns zero penalty when the order has no driver acceptance metadata in `Application/Features/Orders/Services/RefundPenaltyCalculationService.cs`
- [X] T025 [US3] Add a unit test for no-penalty scenarios in `Tests/Application/Features/Orders/Services/RefundPenaltyCalculationServiceTests.cs`
- [X] T026 [US3] Add logging for no-penalty decisions in `Application/Features/Orders/Consumers/OrderCanceledPaymentRefundConsumer.cs`

**Checkpoint**: User Story 3 should be independently testable after these tasks

---

## Phase 6: User Story 4 - Log and Track Penalty Calculations (Priority: P2)

**Goal**: Record penalty calculation details for audit and troubleshooting

**Independent Test**: Verify logs contain original amount, penalty percentage, refund amount, and distance ratio for a processed event.

- [X] T027 [US4] Log `OrderId`, original amount, penalty percentage, refund amount, and distance ratio in `OrderCanceledPaymentRefundConsumer.cs`
- [X] T028 [US4] Add audit-specific log messages in `Application/Features/Orders/Consumers/OrderCanceledPaymentRefundConsumer.cs`
- [X] T029 [US4] Add a consumer unit test to assert logging when penalty calculation occurs in `Tests/Application/Features/Orders/Consumers/OrderCanceledPaymentRefundConsumerTests.cs`
- [X] T030 [US4] Add a unit test for duplicate message handling or idempotent processing in `Tests/Application/Features/Orders/Consumers/OrderCanceledPaymentRefundConsumerTests.cs`

**Checkpoint**: User Story 4 should be independently testable after these tasks

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Finalize documentation, tests, and configuration for the complete feature

- [X] T031 [P] Update `specs/004-refund-penalty-consumer/quickstart.md` with final consumer setup instructions
- [X] T032 [P] Validate the RabbitMQ configuration and consumer registration in `Api/appsettings.Development.json`, `Api/appsettings.json`, and `Application/Shared/Configurations/DependencyInjection.cs`
- [X] T033 [P] Add additional unit tests for invalid payload handling in `Tests/Application/Features/Orders/Consumers/OrderCanceledPaymentRefundConsumerTests.cs`
- [X] T034 [P] Update `Docs/ORDER-CANCELLATION-FLOW.md` with a brief note about the refund penalty consumer
- [X] T035 [ ] Review and refactor `Application/Features/Orders/Consumers/OrderCanceledPaymentRefundConsumer.cs` and `Application/Features/Orders/Services/RefundPenaltyCalculationService.cs`
- [X] T036 [ ] Confirm the feature is compliant with `specs/004-refund-penalty-consumer/spec.md` acceptance criteria
- [ ] T029 [US4] Add a consumer unit test to assert logging when penalty calculation occurs in `Tests/Application/Features/Orders/Consumers/OrderCanceledPaymentRefundConsumerTests.cs`
- [ ] T030 [US4] Add a unit test for duplicate message handling or idempotent processing in `Tests/Application/Features/Orders/Consumers/OrderCanceledPaymentRefundConsumerTests.cs`

**Checkpoint**: User Story 4 should be independently testable after these tasks

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Finalize documentation, tests, and configuration for the complete feature

- [ ] T031 [P] Update `specs/004-refund-penalty-consumer/quickstart.md` with final consumer setup instructions
- [ ] T032 [P] Validate the RabbitMQ configuration and consumer registration in `Api/appsettings.Development.json`, `Api/appsettings.json`, and `Application/Shared/Configurations/DependencyInjection.cs`
- [ ] T033 [P] Add additional unit tests for invalid payload handling in `Tests/Application/Features/Orders/Consumers/OrderCanceledPaymentRefundConsumerTests.cs`
- [ ] T034 [P] Update `Docs/ORDER-CANCELLATION-FLOW.md` with a brief note about the refund penalty consumer
- [ ] T035 [ ] Review and refactor `Application/Features/Orders/Consumers/OrderCanceledPaymentRefundConsumer.cs` and `Application/Features/Orders/Services/RefundPenaltyCalculationService.cs`
- [ ] T036 [ ] Confirm the feature is compliant with `specs/004-refund-penalty-consumer/spec.md` acceptance criteria

---

## Dependencies & Execution Order

- **Phase 1**: Setup tasks can start immediately and are mostly parallelizable.
- **Phase 2**: Foundational tasks block user story development and should complete before story implementation.
- **Phase 3+**: User stories can then be implemented independently, with priority order P1 first.
- **Phase 7**: Polish depends on completion of all user story phases.

## Parallel Opportunities

- Phase 1 tasks T001-T005 can run in parallel because they touch different files.
- Phase 2 tasks T006-T011 can run in parallel where they do not depend on each other.
- User Story phases can proceed in parallel once the foundational phase is complete.
- Logging and documentation tasks in Phase 7 are parallelizable across different files.