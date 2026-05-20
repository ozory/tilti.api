# Fluxo Unificado de Pagamento, Cancelamento e Reembolso

## Objetivo

Documentar o fluxo completo de pagamento de corrida de passageiro, cancelamento por parte do cliente e reembolso via PIX, de forma clara e objetiva. Este documento deve servir como base para futura extração dessa funcionalidade para um microserviço dedicado.

## Escopo

Inclui:
- Precificação de corrida
- Criação/recuperação de carteira do passageiro no provedor de pagamentos
- Processamento de pagamento via Asaas
- Criação de pedido somente após pagamento confirmado
- Cancelamento de pedido pelo passageiro
- Cálculo de penalidade para reembolso parcial quando aplicável
- Transferência de reembolso via PIX para a WalletId do cliente

Não inclui:
- Matching de motoristas
- Entrega de corridas concluídas
- Assinaturas de motoristas

## Visão Geral do Fluxo

1. Passageiro solicita precificação.
2. Sistema calcula e retorna estimativa de valor.
3. Passageiro inicia pagamento.
4. Sistema cria/recupera carteira do passageiro no Asaas.
5. Sistema solicita pagamento ao Asaas e retorna link/QR para o passageiro.
6. Asaas confirma o pagamento via webhook.
7. Pedido é criado/confirmado somente após pagamento aprovado.
8. Passageiro pode cancelar o pedido antes da corrida entrar em andamento.
9. Se cancelado pelo passageiro, sistema publica evento de cancelamento.
10. Consumer de pagamento calcula eventual penalidade.
11. Consumer de reembolso inicia PIX para o cliente com o valor devido.

## Componentes Principais

### Serviços

- `PaymentService` / `PassengerPaymentService`
  - Cria ou recupera carteira do passageiro no Asaas.
  - Gera link de pagamento PIX.
  - Registra pagamento pendente.

- `OrderService`
  - Valida que pedido só é criado após pagamento confirmado.
  - Executa transição de estado do pedido.

- `CancellationService`
  - Valida regras de cancelamento.
  - Atualiza pedido para `Cancelada`.
  - Identifica se cancelamento foi iniciado por usuário ou motorista.
  - Emite evento de cancelamento quando iniciado pelo passageiro.

- `PenaltyService`
  - Calcula a penalidade com base na distância do motorista ao cliente no momento da aceitação.
  - Aplica limite máximo de 80% de penalidade.
  - Retorna valor de reembolso mínimo de 20% quando aplicável.

- `RefundService` / `PixTransferService`
  - Valida WalletId do cliente.
  - Inicia transferência PIX da carteira da Tilt para o cliente.
  - Persistem transações de reembolso.
  - Realiza retry em falhas transitórias até 3 tentativas.

### Eventos e Mensageria

- `OrderCanceledPaymentRefund`
  - Emissão: `CancelOrderCommandHandler` quando o passageiro cancela.
  - Payload sugerido: `OrderId`, `UserId`, `Amount`, `Reason`, `Description`, `CancellationTime`, `CancelledBy`.

- `RefundPixTransferCompletedEvent`
  - Emissão: após PIX concluído com sucesso.

- `RefundPixTransferFailedEvent`
  - Emissão: após todas as tentativas de PIX falharem.

### Webhook

- `WebhookEndpoint` para receber notificações do Asaas.
- Deve extrair `externalReference` e atualizar o pagamento interno.
- Deve disparar `CreateOrderCommand` ou transição similar após confirmação.

## Regras de Negócio

### Pagamento

- Um pedido só deve ser criado quando houver confirmação de pagamento.
- Pagamentos devem ser vinculados a um pedido ou a um identificador de pré-pedido.
- Pagamentos não confirmados não podem gerar pedidos.
- Se o passageiro não tiver carteira no Asaas, ela deve ser criada automaticamente.

### Cancelamento

- Não permitir cancelamento se a corrida já estiver `InProgress`.
- Se nenhuma aceitação de motorista ocorreu, cancelar deve resultar em reembolso integral.
- Se cancelado pelo passageiro, registrar `CancelledBy = User` e publicar evento de reembolso.
- No pedido, armazenar `CancelledBy`, `CancelReason` e `CancelDescription`.

### Penalidade

- Se motorista já aceitou a corrida, aplicar penalidade baseada em distância.
- Penalidade máxima de 80% do valor da corrida.
- Reembolso mínimo de 20% do valor original.
- Não aplicar penalidade quando não houve aceitação de motorista.

### Reembolso PIX

- Transferir via PIX apenas se o valor de reembolso for maior que zero.
- Validar a WalletId do cliente antes de iniciar a transferência.
- Registrar um `RefundTransaction` com status `Pending`, `Completed` ou `Failed`.
- Tentar novamente em falhas transitórias até 3 vezes.

## Modelos de Dados Relevantes

### Order

Campos importantes:
- `Id`
- `PassengerId`
- `Status` (`Pending`, `ReadyToAccept`, `Accepted`, `InProgress`, `Cancelled`, etc.)
- `PaymentId`
- `CancelledBy`
- `CancelReason`
- `CancelDescription`
- `AcceptedAt` / `DriverAcceptedLocation`

### Payment

Campos importantes:
- `Id`
- `OrderId`
- `Amount`
- `Status` (`Pending`, `Approved`, `Cancelled`)
- `AsaasPaymentId`
- `AsaasPaymentLink`
- `AsaasQrCode`
- `PaidAt`

### RefundTransaction

Campos importantes:
- `TransactionId`
- `OrderId`
- `Amount`
- `CustomerWalletId`
- `Status` (`Pending`, `Completed`, `Failed`)
- `RetryCount`
- `CreatedAt`
- `UpdatedAt`

## APIs / Endpoints Sugeridos

- `POST /orders/precify`
  - Solicita estimativa de preço.

- `POST /payments/passenger/create`
  - Inicia pagamento e retorna link/QR.

- `POST /payments/confirm`
  - Sugestão de endpoint para confirmar pagamento e criar/atualizar pedido após validação do pagamento.
  - Pode ser implementado internamente via webhook do Asaas em vez de chamado diretamente pelo cliente.

- `POST /orders/{orderId}/cancel`
  - Cancela pedido pelo passageiro.

- `POST /webhooks/asaas`
  - Recebe notificações de pagamento do Asaas.

## Critérios de Sucesso

- O fluxo completo funciona do preço ao pedido confirmado sem criar pedidos sem pagamento.
- Cancelamento do passageiro antes de `InProgress` atualiza o pedido para `Cancelled` e dispara reembolso.
- Penalidade é aplicada apenas quando há aceitação de motorista.
- Reembolso via PIX ocorre somente com valor positivo e WalletId válido.
- Falhas na transferência PIX são tratadas com retry e registro de erro.

## Considerações para Microserviço

### Separação de responsabilidades

Idealmente, o microserviço deve concentrar:
- pagamentos de passageiros
- cancelamentos relacionados a pagamento
- cálculo de penalidade
- envio de reembolso PIX
- integração com Asaas e mensageria de eventos

### Fronteiras do microserviço

Deve expor APIs para:
- iniciar pagamento
- confirmar pagamento
- cancelar pedido
- receber webhook do Asaas

Deve consumir eventos de domínio como:
- `OrderCanceledPaymentRefund`
- `OrderAccepted` ou similar, se necessário para preencher dados de aceitação

### Persistência

Dados que devem ser mantidos pelo microserviço:
- pagamentos de passageiros
- reembolsos e transações PIX
- estado de cancelamento de pedidos relacionados a pagamento

### Comunicação entre serviços

- Use eventos assíncronos para sinalizar cancelamentos e reembolsos.
- Use `OrderId` como identificador compartilhado entre serviços.
- Mantenha o contrato do evento estável para permitir versionamento.

## Resumo de Sequência

1. `POST /orders/precify` → cálculo de preço.
2. `POST /payments/passenger/create` → cria pagamento Asaas e retorna link/QR.
3. Asaas webhook `POST /webhooks/asaas` → confirma pagamento.
4. Pedido é criado/atualizado com status `ReadyToAccept`.
5. Passageiro chama `POST /orders/{orderId}/cancel`.
6. Pedido muda para `Cancelled`; se cancelado pelo passageiro, publica `OrderCanceledPaymentRefund`.
7. Consumer calcula penalidade e define valor final de reembolso.
8. Consumer de reembolso PIX inicia `RefundTransaction` e chama PIX.
9. Evento `RefundPixTransferCompletedEvent` ou `RefundPixTransferFailedEvent` é publicado.

## Recomendações

- Use contratos de evento bem definidos para isolar o microserviço.
- Mantenha validações de negócio no domínio do microserviço, não apenas em camadas de aplicação.
- Trate idempotência em consumidores e webhooks.
- Logue todas as etapas de pagamento, cancelamento e reembolso para auditoria.
- Garanta que a criação de pedidos dependa exclusivamente da confirmação de pagamento.
