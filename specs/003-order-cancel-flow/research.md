# Research: Cancel Order Flow (003-order-cancel-flow)

## Decisions Made

### 1. Driver Acceptance Representation
**Decision**: The system will use an `AcceptOrderCommand` that sets the order status to `Accepted` and assigns the `DriverId` when a driver accepts an order.
**Rationale**: This approach aligns with the existing CQRS pattern in the codebase and provides a clear audit trail of when drivers accept orders. The `Order` entity will have a `DriverId` property to track which driver accepted the order.
**Alternatives Considered**: 
- Using a boolean `IsDriverAssigned` field (rejected as less informative)
- Storing driver acceptance in a separate table (rejected as over-engineering for this use case)

### 2. OrderCanceledPaymentRefund Message Payload
**Decision**: The message will contain `OrderId`, `userId`, `Amount`, `Reason`, `Description`, and `CancellationTime`.
**Rationale**: This provides all necessary information for the payment processing service to initiate a refund without needing to make additional service calls. Including the cancellation time helps with audit trails and potential time-based refund policies.
**Alternatives Considered**:
- Only including OrderId and letting the payment service fetch details (rejected as creates tight coupling and potential consistency issues)
- Including full order object (rejected as unnecessarily large and potentially exposes sensitive data)

### 3. Cancellation Reason Storage Format
**Decision**: Maintain both `Reason` (as a string for tagging/categorization) and `Description` (as a optional string for detailed explanation) fields in the Order entity.
**Rationale**: This provides flexibility for both structured reporting (using Reason as tags/categories) and detailed customer feedback (using Description). The spec indicates that `reason` will function as tags and `description` can be null but may be specified by the client.
**Alternatives Considered**:
- Single string field (rejected as loses either categorization or detail capability)
- List of strings for Reason (rejected as over-complex for current requirements; can be evolved later if needed)

## Technical Findings

### Existing Patterns in Codebase
- All Command Handlers use `ICommandHandler<TCommand, TResponse>` interface
- All Query Handlers use `IQueryHandler<TQuery, TResponse>` interface
- MediatR is strictly prohibited
- Handlers return `Task<Result<TResponse>>` using FluentResults
- Logging follows pattern: `private readonly string _className = nameof(HandlerName);`
- Validation uses FluentValidation with automatic validation result checking
- Repository pattern is used for data access

### RabbitMQ Integration
- The project already uses RabbitMQ for inter-service communication
- Message publishing follows established patterns in the codebase
- Payment service already has consumers for order-related events

### Order Entity Analysis
Based on similar features (like the passenger payment flow), the Order entity likely contains:
- Id (identifier)
- UserId (customer who placed the order)
- Status (OrderStatus enum)
- DriverId (optional, when assigned)
- Amount (decimal)
- CreatedAt/UpdatedAt timestamps
- We need to add: CancelledBy, CancelReason, CancelDescription, CancelledAt

## Open Questions Requiring Clarification
None - all clarifications from the spec have been addressed in the decisions above.