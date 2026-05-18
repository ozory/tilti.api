# Data Model: Refund Penalty Consumer

## Order

The consumer depends on the existing `Order` aggregate. For this feature, the relevant fields are:

- `Id` (long)
- `DriverId` (long?)
- `Status` (`OrderStatus`)
- `Amount` (`Money` or decimal)
- `CancelledBy` (`CancellationInitiator`)
- `CancelDescription` (string?)
- `CancelReasons` (string?)
- `PickupLocation` (value object or coordinates)
- `DriverLocationAtAcceptance` (value object or coordinates)
- `DriverAcceptanceLocation` (value object or coordinates)
- `TotalDistance` (decimal?)

### Domain assumptions

- A penalty only applies when the order was accepted by a driver.
- The consumer requires a distance ratio that can be derived from driver acceptance data.
- The order acceptance flow must persist the driver's acceptance geolocation so the penalty can be calculated later.
- If the model does not yet include `DriverLocationAtAcceptance`, the service should calculate penalty using a persisted ratio or existing acceptance metadata.

## Domain Event: OrderCanceledPaymentRefundDomainEvent

Represents the event published when a user cancels an order and triggers refund handling.

Fields:
- `OrderId` (long)
- `UserId` (long)
- `Amount` (decimal)
- `Reason` (List<string>)
- `Description` (string)
- `CancellationTime` (DateTime)

## Value Object: PenaltyCalculationResult

Represents the result of the refund penalty calculation.

Fields:
- `OriginalAmount` (decimal)
- `PenaltyPercentage` (decimal)
- `PenaltyAmount` (decimal)
- `RefundAmount` (decimal)
- `DistanceRatio` (decimal)

Rules:
- `PenaltyPercentage` is between 0 and 80.
- `RefundAmount` is `OriginalAmount * (1 - PenaltyPercentage / 100)`.
- `RefundAmount` must never be less than `OriginalAmount * 0.20`.

## Service: RefundPenaltyCalculationService

Responsibilities:
- Calculate penalty percentage from driver distance ratio
- Enforce the maximum 80% penalty rule
- Return a `PenaltyCalculationResult`
- Keep business logic separate from message handling

## Consumer: OrderCanceledPaymentRefundConsumer

Responsibility flow:
1. Receive `OrderCanceledPaymentRefundDomainEvent` from RabbitMQ
2. Validate message payload and order cancellation state
3. Determine whether driver acceptance exists
4. Invoke `RefundPenaltyCalculationService`
5. Log penalty details
6. Avoid refund execution in this iteration

## Message flow

- Source: `CancelOrderCommandHandler` publishes `OrderCanceledPaymentRefundDomainEvent`
- Consumer: `OrderCanceledPaymentRefundConsumer` receives the event
- Output: `PenaltyCalculationResult` logged and ready for future refund execution
