# API Contract: Cancel Order Flow

## Endpoint

**POST** `/api/orders/{orderId}/cancel`

### Description
Cancels an order if it's not already in progress. Only the user who placed the order can cancel it.

### Path Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| orderId   | long | The unique identifier of the order to cancel |

### Request Body

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| UserId | long | Yes | The ID of the user requesting cancellation |
| RequestedTime | DateTime | Yes | Timestamp when cancellation was requested |
| reason | List<string> | Yes | List of cancellation reason tags |
| description | string | No | Detailed description of cancellation reason |
| cancelledBy | string | No | Who initiated cancellation ("User" or "Driver"). Defaults to "User" if not provided |

### Responses

#### Success Response
- **Code**: 200 OK
- **Content**: OrderResponse object representing the cancelled order

#### Error Responses
- **Code**: 400 Bad Request
  - **Content**: Validation errors (invalid input, order not found, etc.)
  
- **Code**: 409 Conflict
  - **Content**: "Pedido já foi cancelado" (Order already cancelled)
  
- **Code**: 400 Bad Request
  - **Content**: "Não é possível cancelar um pedido em andamento" (Cannot cancel an order in progress)
  
- **Code**: 400 Bad Request
  - **Content**: "Pedido já foi Finalizado" (Order already finished)

### RabbitMQ Event

When a user successfully cancels an order, the system publishes an `OrderCanceledPaymentRefund` event to RabbitMQ:

**Exchange**: `OrderCancelledRefund`  
**Routing Key**: `order.cancelled.refund`  
**Queue**: `order.refund.processing`

**Event Payload**:
```json
{
  "OrderId": 123,
  "UserId": 456,
  "Amount": 25.50,
  "Reason": ["user_request"],
  "Description": "Customer changed mind",
  "CancellationTime": "2026-05-17T10:30:00Z"
}
```