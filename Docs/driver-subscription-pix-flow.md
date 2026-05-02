# Fluxo de Assinatura do Motorista via PIX (Asaas)

Este documento detalha o funcionamento técnico da integração com o Asaas para assinaturas de motoristas, garantindo que motoristas só recebam corridas se estiverem com a assinatura ativa.

## Arquitetura do Fluxo

O sistema utiliza o Asaas como gateway de pagamento, focando na modalidade **PIX Recorrente**.

### 1. Criação da Assinatura
O motorista inicia o processo através do endpoint `POST /subscriptions/driver`.

- **Handler**: `CreateDriverSubscriptionCommandHandler`
- **Ação**: 
  - Verifica se o motorista já possui assinatura ativa.
  - Cria um registro local `DriverSubscription` com status `PendingApproval`.
  - Chama o `IDriverPaymentService` para criar a assinatura no Asaas.
  - **Importante**: Enviamos o ID interno da nossa assinatura no campo `externalReference` do Asaas. Isso permite que o webhook nos localize de forma eficiente.
  - Retorna o `AsaasPaymentLink` para o motorista realizar o pagamento.

### 2. Confirmação de Pagamento (Webhook)
O Asaas notifica nosso backend através do webhook `POST /webhooks/asaas`.

- **Endpoint**: `WebhookEndpoint.cs`
- **Eventos Processados**: `PAYMENT_RECEIVED` ou `PAYMENT_CONFIRMED`.
- **Lógica**:
  - Extrai o `externalReference` (nosso ID interno) do payload do pagamento.
  - Dispara o `ActivateDriverSubscriptionCommand`.
  - O handler `ActivateDriverSubscriptionCommandHandler` marca a assinatura como `Active` e registra a data de pagamento.
  - Se for uma renovação, a `DueDate` é atualizada com base no próximo vencimento informado pelo Asaas.

### 3. Expiração e Cancelamento
Caso o pagamento não seja identificado ou a assinatura seja cancelada no Asaas.

- **Eventos**: `PAYMENT_EXPIRED` ou `PAYMENT_CANCELED`.
- **Lógica**:
  - Identifica a assinatura pelo `AsaasSubscriptionId`.
  - Dispara o `InactivateDriverSubscriptionCommand`.
  - O motorista perde o acesso ao recebimento de corridas imediatamente.

## Componentes Técnicos

### Entidades
- **DriverSubscription**: Armazena o vínculo entre usuário, plano e IDs externos (Asaas). Possui métodos como `MarkAsPaid()`, `SetStatus()` e `SetDueDate()`.

### Serviços
- **IDriverPaymentService**: Abstração para chamadas HTTP ao Asaas.
- **DriverPaymentService**: Implementação utilizando `RestSharp` para comunicação com a API v3 do Asaas.

### Repositórios
- **IDriverSubscriptionRepository**: Possui métodos especializados como `GetByAsaasPaymentId` e `GetByAsaasSubscriptionId`.

## Configuração (appsettings.json)
```json
{
  "Configurations": {
    "PaymentToken": "...",
    "PaymentUrl": "https://api-sandbox.asaas.com",
    "WalletId": "..."
  }
}
```

## Como Testar (Sandbox)
1. Crie uma assinatura via API.
2. Acesse o painel do Asaas Sandbox.
3. Localize a cobrança criada.
4. Utilize a ferramenta de simulador de Webhook do Asaas ou marque a cobrança como paga manualmente no painel para disparar a notificação.
5. Verifique o status da assinatura no banco de dados Tilt.
