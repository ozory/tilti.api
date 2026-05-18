# Feature Specification: Cancel Order Flow (003-order-cancel-flow)

**Feature Branch**: [003-order-cancel-flow]  
**Created**: May 17, 2026  
**Status**: Draft  
**Input**: User description: "Avaliando a aplicação, vamos tentar seguir com o fluxo de cancelamento. Primeiro, cancelando pelo cliente, quando o cliente cancelar, precisamos validar os seguintes items: - Se a corrida já estiver em andamento, não é possível cancelar. - Se nenhum motorista aceitou a corrida, mas ela não está ainda em andamento, então o valor total é retornado ao cliente. E a Order precisa ter o seu status como "Cancelada". Devemos no CancelOrderCommand também identificar QUEM cancelou a order, se o user ou o driver. Caso tenha sido o user, o cancelamento deve gerar uma mensagem no rabbitmq: OrderCanceledPaymentRefound, qual deve ser interceptada por um consumer no Payment. E apartir dai iniciar o reembolso ao cliente."

## User Scenarios & Testing

### User Story 1 - Cancel Order Before Driver Acceptance (Priority: P1)

**Why this priority**: This is the core cancellation flow that directly impacts user experience and revenue handling. Users expect to cancel orders before they are matched with a driver, and the system must handle refunds correctly.

**Independent Test**: Can be tested by creating an order with status "Pending" and no driver assigned, then issuing a cancel request. The system should return a successful cancellation, set order status to "Cancelada", and emit a "OrderCanceledPaymentRefund" event.

**Acceptance Scenarios**:
1. Given an order exists with status "Pending" and no driver assigned, when a user issues a cancel request, then the order status should be updated to "Cancelada".
2. Given the order status is updated to "Cancelada", when the cancellation is processed, then a "OrderCanceledPaymentRefund" message should be published to RabbitMQ.
3. Given the cancellation is processed, then the system should record that the user initiated the cancellation.

### User Story 2 - Prevent Cancellation During Active Ride (Priority: P1)

**Why this priority**: Allowing cancellation of active rides would lead to inconsistent state and potential financial loss. Preventing this ensures system integrity.

**Independent Test**: Can be tested by creating an order with status "InProgress" and attempting to cancel. The system should reject the cancellation with an appropriate error.

**Acceptance Scenarios**:
1. Given an order exists with status "InProgress", when a cancel request is issued, then the system should return a failure indicating cancellation is not allowed.
2. The error message should indicate that the ride is already in progress.

### User Story 3 - Identify Who Initiated Cancellation (Priority: P2)

**Why this priority**: The system needs to know whether the cancellation was initiated by the user or driver to handle downstream processes correctly.

**Independent Test**: Can be tested by issuing a cancel request and checking the "CancelledBy" field in the order record.

**Acceptance Scenarios**:
1. Given a cancellation request, then the order should have a "CancelledBy" property set to "User" or "Driver" based on the request.
2. This information should be persisted in the order entity.

## Functional Requirements

- **FR1**: Validate that cancellation is only allowed when order status is not "InProgress".
- **FR2**: Set order status to "Cancelada" when cancellation is successful.
- **FR3**: Populate the "CancelledBy" field with "User" when the user initiates cancellation.
- **FR4**: Publish a "OrderCanceledPaymentRefund" message to RabbitMQ when a user cancels an order.
- **FR5**: Persist cancellation reason and description for audit purposes.
- **FR6**: Return appropriate error messages to the client for invalid cancellation attempts.

## Success Criteria

- **Quantitative**: 
  - Cancellation request is processed within 500ms for 95% of requests.
  - "OrderCanceledPaymentRefund" message is published for 100% of user-initiated cancellations.
  - System rejects at least 99% of cancellation attempts on "InProgress" orders.

- **Qualitative**: 
  - Users can successfully cancel orders before driver acceptance with clear feedback.
  - System maintains consistent order state transitions.
  - Downstream payment processing correctly handles refund triggers.

## Key Entities

- **Order**: Entity representing a ride order, containing fields: Status, CancelledBy, CancelDescription, CancelRasons.
- **CancelOrderCommand**: Command containing userId, orderId, requestedTime, reason, description.
- **CancelOrderCommandHandler**: Handler implementing ICommandHandler<CancelOrderCommand, OrderResponse>.
- **OrderStatus**: Enum with values including Pending, InProgress, Cancelled, Finished.
- **RabbitMQ**: External messaging system for publishing "OrderCanceledPaymentRefund" messages.

## Assumptions

- The Order entity has a `CancelledBy` property (string) to track who initiated cancellation.
- The Order entity has `CancelDescription` and `CancelRasons` fields for audit.
- The system uses RabbitMQ for event publishing; a consumer in the Payment module will handle "OrderCanceledPaymentRefund".
- "InProgress" status is represented by `OrderStatus.InProgress`.
- If no driver has accepted the order, the order status will be "Pending" or similar.

## Clarifications

- **Clarification 1**: How is "driver accepted the ride" represented in the current domain model? 
  - *Answer*: We will create an `AcceptOrderCommand` that analyzes if the ride is not in progress or not cancelled. If available, it will change the status to `Accepted` and set the `DriverId` of the driver who accepted.

- **Clarification 2**: What is the exact payload format for the "OrderCanceledPaymentRefund" message? 
  - *Answer*: The message will contain `OrderId`, `userId`, `Amount`, `Reason`, `Description`, and `CancellationTime`.

- **Clarification 3**: Should the cancellation reason be stored as a list of strings or a single string? 
  - *Answer*: We will maintain both fields - `reason` will function as tags (to be defined), and `description` can be null but may be specified by the client to better explain the cancellation reason.