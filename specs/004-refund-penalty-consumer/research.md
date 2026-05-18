# Research: Refund Penalty Consumer

## 1. Existing architecture

The project already implements RabbitMQ consumers as `BackgroundService` classes in `Application/Features/Orders/Consumers`. These consumers use `IMessageRepository` and `MessageRepository` from `Infrastructure/Messages/MessageRepository.cs` to connect, declare queues, and consume messages.

Decision: Reuse the same consumer model rather than introducing a new messaging framework.

## 2. RabbitMQ consumer pattern

The existing `OrderFinishedConsumer` shows the current convention:
- `IMessageRepository.GetConnectionFactory()` to create a connection
- `IMessageRepository.StartNewChannel(queueName)` to create a RabbitMQ channel
- `repo.ConsumeAsync<T>(sharedChannel, action)` to register an async handler
- Hosted service registration with `services.AddHostedService<ConsumerClass>()`

Decision: Implement `OrderCanceledPaymentRefundConsumer` using the same `BackgroundService` and `IMessageRepository` flow.

## 3. Event contract and payload

The domain event already exists as `OrderCanceledPaymentRefundDomainEvent` in `Domain/Features/Orders/Events`:
- `OrderId` (long)
- `UserId` (long)
- `Amount` (decimal)
- `Reason` (List<string>)
- `Description` (string)
- `CancellationTime` (DateTime)

Decision: Use the existing domain event payload as the consumer contract and add a dedicated contract document for clarity.

## 4. Penalty calculation approach

The requested business rule is:
- Penalty depends on driver distance ratio when the ride was accepted
- Penalty increases as the driver approaches the customer
- Maximum penalty is 80%, minimum refund is 20%
- If the driver never accepted the ride, no penalty applies

Alternative considered: computing penalty inline inside the consumer.
Decision: extract the rule into a dedicated `RefundPenaltyCalculationService` to keep the consumer focused on message handling and validation.

## 5. Scope and non-goals

This feature intentionally does not execute the refund. It only:
- receives and validates the cancellation event
- confirms the order was cancelled
- invokes penalty calculation logic
- logs audit details

Non-goal: no actual payment reversal or refund API call in this iteration.

## 6. Configuration

The consumer will reuse conventions from existing message config keys, using a new section for order cancellation refund messages:
- `Infrastructure:OrderCanceledPaymentRefundMessages:queue`
- `Infrastructure:OrderCanceledPaymentRefundMessages:consumerIntances`
- `Infrastructure:OrderCanceledPaymentRefundMessages:delayInterval`

Decision: Keep queue configuration consistent with other consumers and avoid hardcoded RabbitMQ connection strings in code.
