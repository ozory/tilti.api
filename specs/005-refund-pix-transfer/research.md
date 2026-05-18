# Research: Refund via PIX Transfer

**Date**: 2026-05-17
**Spec**: [spec.md](spec.md)

## Clarifications Resolved

All questions from the specification were clarified during the `/speckit.clarify` session.

### Decision 1: PIX Transfer Execution Pattern
- **Decision**: Synchronous PIX transfer - call PIX API and wait for response before completing.
- **Rationale**: The spec mentions a "configured PIX integration service that can be called synchronously" and the 2-minute SLA requirement favors immediate feedback.
- **Alternatives considered**: Asynchronous with callback, hybrid approach.

### Decision 2: Retry Interval Strategy
- **Decision**: 30-second exponential backoff (30s, 60s, 120s).
- **Rationale**: Provides reasonable retry intervals that balance quick recovery with not overwhelming the PIX service, while staying within the 2-minute SLA for 95% of transfers.
- **Alternatives considered**: Fixed 5-second intervals, 1-minute exponential backoff.

### Decision 3: Refund Transaction Persistence
- **Decision**: RefundTransaction entity with IRefundTransactionRepository.
- **Rationale**: Follows the existing CQRS pattern in the codebase (similar to Order, Subscription entities) and provides proper audit logging with persistence.
- **Alternatives considered**: In-memory logging only, event sourcing.

### Decision 4: PIX Provider
- **Decision**: Asaas PIX API - use existing Asaas integration.
- **Rationale**: Aligns with the existing Asaas integration mentioned in copilot-instructions.md (AsaasPaymentId, AsaasPaymentLink, AsaasSubscriptionId) and maintains consistency with the current payment infrastructure.
- **Alternatives considered**: Gerencianet PIX API, manual PIX via bank portal.

### Decision 5: Consumer Extension Approach
- **Decision**: Extend existing `OrderCanceledPaymentRefundDomainEventHandler` to call PIX service.
- **Rationale**: Follows the existing pattern mentioned in the spec's Assumptions section and maintains consistency with the established architecture.
- **Alternatives considered**: New separate command handler, separate background service.

## Technical Research

### Existing Patterns in Codebase

1. **Command Handler Pattern** (`Application/Features/Orders/Commands/Cancel/CancelOrderCommandHandler.cs`):
   - Uses `ICommandHandler<TCommand, TResponse>` interface
   - Returns `Task<Result<TResponse>>` using FluentResults
   - Injects dependencies via constructor
   - Logs with className pattern

2. **Consumer Pattern** (`Application/Features/Orders/Consumers/OrderCanceledPaymentRefundConsumer.cs`):
   - BackgroundService that consumes RabbitMQ messages
   - Uses `IServiceScopeFactory` for scoped dependencies
   - Publishes domain events after processing

3. **Event Pattern** (`Domain/Features/Orders/Events/OrderCanceledPaymentRefundDomainEvent.cs`):
   - Simple DTO with OrderId, Amount, UserId properties
   - Used for inter-service communication via RabbitMQ

### PIX Integration Considerations

Based on the Asaas integration pattern already in use:
- PIX transfers require a valid PIX key (WalletId)
- The WalletId is a random UUID stored in the user profile
- Transfer amount must be positive
- API returns transaction ID for tracking