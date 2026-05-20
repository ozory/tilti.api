# Fluxo Real Existente: Criação de Order, Pagamento e Aceitação

Este diagrama usa apenas os endpoints e handlers presentes no repositório atual.

```mermaid
sequenceDiagram
    participant Cliente as Cliente/API
    participant Api as Api
    participant Cmd as CommandHandler
    participant Pagamento as PaymentService
    participant Asaas as Asaas
    participant Repositorio as Repositório

    Cliente->>Api: POST /payments/passenger/create
    Api->>Cmd: CreatePassengerPaymentCommand
    Cmd->>Pagamento: CreateOrGetWalletAsync(UserId)
    Cmd->>Pagamento: CreatePaymentAsync(amount, walletId, dueDate)
    Pagamento-->>Cmd: paymentId, pixLink, qrCode
    Cmd->>Repositorio: Save Payment (Pending)
    Cmd-->>Api: PaymentResponse(pixLink, qrCode, paymentId)

    Cliente->>Api: POST /orders
    Api->>Cmd: CreateOrderCommand(AsaasPaymentId, amount, addresses, userId)
    Cmd->>Pagamento: CheckPaymentStatusAsync(AsaasPaymentId)
    Pagamento-->>Cmd: paymentApproved?
    Cmd->>Repositorio: Save Order(status=ReadyToAccept, paymentId)
    Cmd-->>Api: OrderResponse

    Asaas->>Api: POST /webhooks/asaas
    Api->>Cmd: UpdatePaymentStatusCommand(asaasPaymentId, status)
    Cmd->>Repositorio: Get payment by AsaasPaymentId
    Cmd->>Pagamento: ApprovePayment() / CancelPayment()
    Cmd->>Repositorio: Update payment status

    note right of Api: Existem também os estados de domínio
    note right of Api: `OrderStatus.Accepted` e `Order.DriverId`
    note right of Cmd: O código permite rastreamento
    note right of Cmd: apenas se `Order.Status == Accepted || InTransit`

    Driver->>Api: POST /orders/tracking
    Api->>Cmd: AddTrackingCommand
    Cmd->>Repositorio: valida order.Status e order.DriverId
    Cmd-->>Api: tracking salvo
```

## Observações

- O endpoint `POST /payments/passenger/create` existe no arquivo `Api/Endpoints/PaymentsEndpoint.cs`.
- O endpoint `POST /orders` existe em `Api/Endpoints/OrdersEndpoint.cs`.
- O webhook `POST /webhooks/asaas` existe em `Api/Endpoints/WebhookEndpoint.cs`.
- O projeto atual não expõe um endpoint explícito de "aceitação de motorista"; o domínio já possui o estado `OrderStatus.Accepted` e o `DriverId` no pedido.
- A regra de aceitação implícita é refletida em `Application/Features/Orders/Commands/AddTracking/AddTrackingCommandHandler.cs`, que só permite tracking para pedidos `Accepted` ou `InTransit`.
