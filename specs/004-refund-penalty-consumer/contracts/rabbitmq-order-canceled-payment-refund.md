# Contract: OrderCanceledPaymentRefund RabbitMQ Event

## Event name

`OrderCanceledPaymentRefund` (domain event type: `OrderCanceledPaymentRefundDomainEvent`)

## Exchange / Queue

- Exchange: configured by `Infrastructure:OrderCanceledPaymentRefundMessages:exchange`
- Queue: configured by `Infrastructure:OrderCanceledPaymentRefundMessages:queue`
- Routing key: configured by `Infrastructure:OrderCanceledPaymentRefundMessages:routingKey`

## Payload schema

```json
{
  "OrderId": 123,
  "UserId": 456,
  "Amount": 20.00,
  "Reason": ["customer_request"],
  "Description": "Cancelamento após aceitação do motorista",
  "CancellationTime": "2026-05-17T12:34:56Z"
}
```

### Fields

- `OrderId` (long): identifier of the canceled order
- `UserId` (long): identifier of the customer who requested cancellation
- `Amount` (decimal): total order amount at cancellation time
- `Reason` (List<string>): cancellation reasons or tags
- `Description` (string): optional cancellation description
- `CancellationTime` (DateTime): timestamp of the cancellation event

## Consumer expectations

- The consumer must deserialize the payload into `OrderCanceledPaymentRefundDomainEvent`.
- The consumer must verify the underlying order exists and is in a canceled state.
- If the driver never accepted the order, the penalty must be 0.
- If the driver accepted and moved toward pickup, the penalty should be calculated up to 80%.
- Invalid or malformed payloads must be logged and rejected.
