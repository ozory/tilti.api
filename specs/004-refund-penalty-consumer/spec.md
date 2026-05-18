# Feature Specification: Refund Penalty Consumer

**Feature Branch**: `004-refund-penalty-consumer`  
**Created**: May 17, 2026  
**Status**: Draft  
**Input**: User description: "Partindo do cancelamento, vamos criar um consumer, que irá receber a mensagem de cancelamento da Order, e processar o reembolso ao cliente: Vamos considerar a seguinte regra; precisamos penalisar o cliente, caso o motorista já tenha aceitado a corrida e já esteja indo em direção ao cliente. Quanto mais próximo do cliente, maior será a multa, retornando ao cliente apenas "parte" do valor da corrida. Isso será para desencentivar o cliente a ficar cancelando. Vamos pensar nessa multa da seguinte forma; temos que considerar o local onde o motorista estava quando aceitou a corrida e o local onde o cliente está. Nessa especificação, vamos apenas calcular o valor da multa, por exemplo se a corrida custou 20 reais e o motorista esta na metade da distância, a multa é de 30 ~ 40 porcento. Vamos efetuar uma lógica que será multado até 80% do valor da corrida, mas nunca 100%. Para que o cliente não perca todo o dinheiro e não consiga chamar uma próxima."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Receive Order Canceled Message (Priority: P1)

**Why this priority**: This is the entry point for the refund penalty flow. Without receiving and validating the cancellation message, the system cannot process any refunds.

**Independent Test**: Can be tested by sending a valid `OrderCanceledPaymentRefund` message to the RabbitMQ queue and verifying the consumer receives and validates it.

**Acceptance Scenarios**:
1. Given an `OrderCanceledPaymentRefund` message is published to RabbitMQ, when the consumer receives it, then the system should validate the order exists and is in canceled status.
2. Given a valid cancellation message, when the consumer processes it, then it should invoke the penalty calculation service.
3. Given an invalid or malformed message, when the consumer receives it, then it should log the error and reject the message.

---

### User Story 2 - Calculate Penalty Based on Distance (Priority: P1)

**Why this priority**: This is the core business logic that determines how much the customer is refunded. The penalty calculation directly impacts revenue and customer satisfaction.

**Independent Test**: Can be tested by providing different distance ratios (0%, 25%, 50%, 75%, 100%) and verifying the correct penalty percentage is applied.

**Acceptance Scenarios**:
1. Given a ride cost of $20 and driver at 0% distance from pickup, when penalty is calculated, then customer receives 100% refund (no penalty).
2. Given a ride cost of $20 and driver at 50% distance, when penalty is calculated, then customer receives approximately 60-70% refund (30-40% penalty).
3. Given a ride cost of $20 and driver at 90% distance, when penalty is calculated, then customer receives approximately 20% refund (80% penalty).
4. Given any ride cost, when penalty is calculated, then the maximum penalty is 80% and minimum refund is 20%.

---

### User Story 3 - Reject Orders Without Driver Acceptance (Priority: P2)

**Why this priority**: Orders that were canceled before a driver accepted should not incur any penalty. This protects customers who cancel immediately.

**Independent Test**: Can be tested by sending a cancellation message for an order where no driver accepted, and verifying no penalty is applied.

**Acceptance Scenarios**:
1. Given an order canceled before driver acceptance, when the consumer processes it, then no penalty should be calculated.
2. Given no penalty, when the refund is processed, then the customer receives 100% of the order amount.

---

### User Story 4 - Log and Track Penalty Calculations (Priority: P2)

**Why this priority**: Audit trail is essential for financial operations and dispute resolution.

**Independent Test**: Can be tested by processing a cancellation and verifying the penalty calculation is logged with all relevant details.

**Acceptance Scenarios**:
1. Given a penalty calculation is performed, when the process completes, then the calculation details should be logged.
2. Given a logged calculation, when reviewing logs, then the original amount, penalty percentage, and refund amount should be visible.

---

### Edge Cases

- What happens if the order is not found in the database?
- How does the system handle if the driver location data is missing?
- What happens if the message is redelivered (duplicate processing)?
- How does the system handle if the penalty calculation service is unavailable?

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST receive and validate `OrderCanceledPaymentRefund` messages from RabbitMQ.
- **FR-002**: System MUST validate that the order exists and is in canceled status before processing.
- **FR-003**: System MUST calculate penalty percentage based on driver distance ratio (0-100%).
- **FR-004**: System MUST apply penalty between 0% and 80% based on distance (closer to customer = higher penalty).
- **FR-005**: System MUST ensure minimum refund is 20% of the original order amount (maximum penalty 80%).
- **FR-006**: System MUST NOT apply penalty if no driver accepted the order.
- **FR-007**: System MUST persist the driver's acceptance geolocation when the order is accepted.
- **FR-008**: System MUST log all penalty calculations for audit purposes.
- **FR-009**: System MUST reject and log invalid or malformed messages.
- **FR-010**: System MUST handle duplicate message processing gracefully (idempotency).

### Penalty Calculation Rules

| Driver Distance | Penalty % | Refund % |
|-----------------|-----------|----------|
| 0% (pickup)     | 0%        | 100%     |
| 25%             | 10-15%    | 85-90%   |
| 50% (midpoint)  | 30-40%    | 60-70%   |
| 75%             | 60-70%    | 30-40%   |
| 90%+            | 70-80%    | 20-30%   |

### Key Entities *(include if feature involves data)*

- **OrderCanceledPaymentRefund**: Message containing OrderId, UserId, Amount, Reason, Description, CancellationTime.
- **Order**: Entity with DriverId, Status, Amount, pickup location, dropoff location, and driver acceptance geolocation.
- **PenaltyCalculation**: Value object containing originalAmount, penaltyPercentage, refundAmount.
- **RefundConsumer**: RabbitMQ consumer that processes cancellation messages.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Consumer processes valid cancellation messages within 2 seconds for 95% of requests.
- **SC-002**: Penalty calculation is applied correctly for 100% of orders with driver acceptance.
- **SC-003**: No penalty is applied for 100% of orders canceled before driver acceptance.
- **SC-004**: System handles 100% of duplicate messages without double-charging customers.
- **SC-005**: All penalty calculations are logged with sufficient detail for audit purposes.

## Assumptions

- The `OrderCanceledPaymentRefund` message is published by `CancelOrderCommandHandler` when a user cancels an order.
- Driver distance is calculated as the ratio between distance traveled and total ride distance.
- The consumer will be implemented in the Payment module of the application.
- The system already has access to order details including driver location at the time of cancellation.
- The order acceptance flow must persist the driver's acceptance geolocation so the penalty can be calculated later.
- RabbitMQ is already configured and operational in the infrastructure.
- The penalty calculation logic will be extracted to a separate service for testability.
- No actual refund will be processed in this specification - only the penalty calculation.

## Clarifications

- **Clarification 1**: How is the driver's current location determined at the time of cancellation?
  - *Answer*: The system will use the last known location of the driver when the cancellation message is processed. This may be approximate.

- **Clarification 2**: What happens if the order is canceled by the driver instead of the user?
  - *Answer*: No penalty will be applied when the driver cancels the order. The consumer will check the cancellation initiator.

- **Clarification 3**: Should the penalty calculation be configurable or hardcoded?
  - *Answer*: For this specification, the penalty calculation will be hardcoded with the defined rules. Configuration can be added in future iterations.