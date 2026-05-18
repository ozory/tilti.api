# Quickstart: Refund via PIX Transfer

**Date**: 2026-05-17
**Spec**: [spec.md](spec.md)

## Prerequisites

1. Docker and Docker Compose installed
2. MongoDB running locally (or via Docker)
3. PostgreSQL running locally (or via Docker)
4. RabbitMQ running locally (or via Docker)
5. Asaas API credentials configured in `appsettings.Development.json`

## Running Locally

### 1. Start Infrastructure

```bash
# Start MongoDB
docker run -d --name mongo-local -p 27017:27017 -v mongo-data:/data/db mongo:8

# Start PostgreSQL
docker run -d --name postgres-local -p 5432:5432 -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=tilt postgres:15

# Start RabbitMQ
docker run -d --name rabbitmq-local -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

### 2. Build and Run the API

```bash
dotnet build
dotnet run --project Api/Api.csproj
```

### 3. Trigger a Test Refund

#### Option A: Via API (if order exists)

```bash
# Cancel an order as a client
curl -X PATCH "http://localhost:5000/orders/{orderId}/cancel" \
  -H "Content-Type: application/json" \
  -d '{"userId": 123, "cancelledBy": "User"}'
```

#### Option B: Publish test event to RabbitMQ

```bash
# Use RabbitMQ management UI at http://localhost:15672
# Or use a tool like rabbitmqadmin to publish to the order.canceled.payment.refund queue
```

### 4. Verify the Refund

#### Check MongoDB for RefundTransaction

```bash
# Connect to MongoDB
mongosh

# Query refund transactions
use tilt
db.refund_transactions.find().pretty()
```

#### Check Logs

```bash
# Look for PIX transfer logs
dotnet run | grep -i "pix\|refund"
```

## Expected Flow

1. Order is canceled by client
2. `OrderCanceledPaymentRefundDomainEvent` is published
3. `OrderCanceledPaymentRefundConsumer` processes the event
4. Penalty calculation is performed (if applicable)
5. `RefundPixTransferCommand` is dispatched
6. `RefundPixTransferCommandHandler`:
   - Validates WalletId format
   - Calls Asaas PIX API
   - Persists RefundTransaction
   - Emits success/failure event

## Troubleshooting

### WalletId Validation Fails

- Ensure the user has a valid WalletId (UUID format) in their profile
- Check the user document in MongoDB

### PIX Transfer Fails

- Check Asaas API credentials in configuration
- Verify Tilt's wallet has sufficient balance
- Check network connectivity to Asaas API

### Consumer Not Processing

- Verify RabbitMQ connection
- Check queue name configuration
- Ensure consumer is running (check logs for "Worker's ativo")