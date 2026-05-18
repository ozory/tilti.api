# Tasks: Refund via PIX Transfer

**Input**: Design documents from `/specs/005-refund-pix-transfer/`
**Prerequisites**: plan.md, spec.md

## Phase 1: Setup (Shared Infrastructure)

- [X] T001 [P] Create project structure per implementation plan (`/specs/005-refund-pix-transfer/plan.md`).
- [X] T002 [P] Add `PixTransferService.cs` wrapper in `Infrastructure/Services/`.
- [X] T003 [P] Add `RefundTransactionRepository.cs` in `Infrastructure/Repositories/` for PostgreSQL audit logs.

## Phase 2: Foundational (Blocking Prerequisites)

- [X] T004 Ensure `RefundPenaltyCalculationService` is functional and returns final refundable amount (already exists).
- [X] T005 [P] Create contract events `RefundPixTransferCompletedEvent.cs` and `RefundPixTransferFailedEvent.cs` in `Application/Features/Orders/Events/`.
- [X] T006 Register `PixTransferService` and `RefundTransactionRepository` in DI container (`DependencyInjection.cs`).

## Phase 3: User Story 1 - Refund via PIX after client cancellation (Priority: P1) 🎯 MVP

**Goal**: After a client‑canceled order, automatically transfer the refund via PIX.
**Independent Test**: Cancel an order, verify a PIX transfer is created and audit log entry exists.

- [X] T007 [US1] Create `RefundPixTransferCommand.cs` in `Application/Features/Orders/Commands/`.
- [X] T008 [US1] Create `RefundPixTransferCommandHandler.cs` in `Application/Features/Orders/CommandHandlers/` implementing `ICommandHandler<RefundPixTransferCommand, Result>`.
- [X] T009 [US1] Extend `OrderCanceledPaymentRefundConsumer` to publish `RefundPixTransferCommand` after penalty calculation (`Application/Features/Orders/Consumers/OrderCanceledPaymentRefundConsumer.cs`).
- [X] T010 [US1] Implement `PixTransferService.TransferAsync(Guid transactionId, decimal amount, string walletId)` with call to internal PIX provider.
- [X] T011 [US1] Implement `RefundTransactionRepository` methods: `CreateAsync`, `UpdateStatusAsync`.
- [X] T012 [US1] Write unit test for successful PIX transfer (mock `PixTransferService` and repository).
- [X] T013 [US1] Write unit test for zero‑amount refund – handler should skip transfer.
- [X] T014 [US1] Write unit test for invalid WalletId – handler logs warning and does not call PIX service.
- [ ] T015 [US1] Write integration test exercising full flow: publish `OrderCanceledPaymentRefundDomainEvent`, verify `RefundPixTransferCompletedEvent` emitted and audit log entry created.

## Phase 4: User Story 2 - Handle invalid or missing WalletId (Priority: P2)

**Goal**: Prevent transfer when the customer's WalletId is missing or malformed.
**Independent Test**: Cancel an order with an invalid WalletId and verify no PIX call is made and an error is logged.

- [X] T016 [US2] Add validation in `RefundPixTransferCommandHandler` to check WalletId format (UUID).
- [X] T017 [US2] Ensure handler logs a warning and returns `Result.Fail` when WalletId is invalid.
- [X] T018 [US2] Write unit test for invalid WalletId scenario confirming no PIX call and proper logging.

## Phase 5: User Story 3 - Retry on transient PIX failures (Priority: P3)

**Goal**: Retry up to three times on transient errors before marking the refund as failed.
**Independent Test**: Simulate a transient failure on first attempt, ensure retry succeeds; simulate persistent failure, ensure `RefundPixTransferFailedEvent` is emitted.

- [X] T019 [US3] Implement retry logic with exponential back‑off (max 3 attempts) in `RefundPixTransferCommandHandler`.
- [X] T020 [US3] Write unit test where first call to `PixTransferService` throws transient exception and second call succeeds – verify transaction marked Completed.
- [X] T021 [US3] Write unit test where all three attempts fail – verify `RefundPixTransferFailedEvent` emitted and transaction status set to Failed.

## Phase 6: Polish & Cross‑Cutting Concerns

- [ ] T022 Update README and documentation with new refund‑via‑PIX flow.
- [ ] T023 Add audit‑log verification step in quickstart (`quickstart.md`).
- [ ] T024 Ensure all new classes are covered by unit tests (≥80% coverage).
- [ ] T025 Run static analysis and fix any warnings.

---

**Dependencies**
- Phase 1 must complete before Phase 2.
- Phase 2 must complete before any user‑story phases.
- User Story phases can run in parallel after Phase 2 is done.

**Parallel Opportunities**
- Tasks marked `[P]` can be executed concurrently.
- Different user stories (`[US1]`, `[US2]`, `[US3]`) can be worked on by separate developers once Phase 2 is finished.