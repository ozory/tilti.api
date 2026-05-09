# Asaas Subscription Creation

**WORKFLOW SKILL** — Criar assinaturas recorrentes via API Asaas. Use para implementar fluxos de assinatura para motoristas e passageiros.

## Quando Usar

- Criar nova assinatura para motorista
- Criar assinatura para passageiro
- Configurar cobrança recorrente PIX
- Gerar link de pagamento

## Payload Mínimo para Assinatura

### PIX Recorrente (Recomendado)
```json
{
  "customer": "cus_XXXXXXXX",
  "billingType": "PIX",
  "value": 49.90,
  "cycle": "MONTHLY",
  "nextDueDate": "2026-06-09",
  "description": "Assinatura Motorista - Plano Pro",
  "externalReference": "sub_internal_id",
  "walletId": "wallet_id_asaas"
}
```

### Com Cartão de Crédito
```json
{
  "customer": "cus_XXXXXXXX",
  "billingType": "CREDIT_CARD",
  "value": 49.90,
  "cycle": "MONTHLY",
  "nextDueDate": "2026-06-09",
  "creditCard": {
    "holderName": "Nome no Cartão",
    "number": "4111111111111111",
    "expiryMonth": "12",
    "expiryYear": "2026",
    "ccv": "123"
  },
  "creditCardHolderInfo": {
    "name": "Nome Completo",
    "email": "email@exemplo.com",
    "cpfCnpj": "12345678901",
    "postalCode": "01234567",
    "addressNumber": "123"
  }
}
```

## Implementação CQRS

### Command
```csharp
public record CreateSubscriptionCommand(
    string CustomerId,
    decimal Value,
    string Cycle,
    DateTime NextDueDate,
    string Description,
    string ExternalReference,
    string WalletId,
    string BillingType = "PIX"
) : ICommand<SubscriptionResponse>;
```

### Handler
```csharp
public class CreateSubscriptionCommandHandler : ICommandHandler<CreateSubscriptionCommand, SubscriptionResponse>
{
    private readonly IAsaasService _asaasService;
    private readonly ISubscriptionRepository _repository;
    private readonly ILogger<CreateSubscriptionCommandHandler> _logger;
    private readonly string _className = nameof(CreateSubscriptionCommandHandler);

    public async Task<Result<SubscriptionResponse>> Handle(
        CreateSubscriptionCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("[{className}] Creating subscription for customer {CustomerId}", 
                _className, request.CustomerId);

            var asaasRequest = new AsaasSubscriptionRequest
            {
                Customer = request.CustomerId,
                BillingType = request.BillingType,
                Value = request.Value,
                Cycle = request.Cycle,
                NextDueDate = request.NextDueDate.ToString("yyyy-MM-dd"),
                Description = request.Description,
                ExternalReference = request.ExternalReference,
                WalletId = request.WalletId
            };

            var asaasResponse = await _asaasService.CreateSubscriptionAsync(asaasRequest);

            var subscription = Subscription.Create(
                customerId: request.CustomerId,
                value: request.Value,
                cycle: request.Cycle,
                nextDueDate: request.NextDueDate,
                asaasSubscriptionId: asaasResponse.Id,
                asaasPaymentLink: asaasResponse.PaymentLink,
                description: request.Description
            );

            await _repository.SaveAsync(subscription);

            return Result.Ok((SubscriptionResponse)subscription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{className}] Error creating subscription", _className);
            return Result.Fail($"Failed to create subscription: {ex.Message}");
        }
    }
}
```

## Campos Obrigatórios

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `customer` | string | ID do cliente no Asaas |
| `value` | number | Valor da assinatura |
| `cycle` | string | Frequência (MONTHLY, WEEKLY, etc) |
| `nextDueDate` | string | Data do próximo pagamento (yyyy-MM-dd) |

## Campos Recomendados

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `externalReference` | string | ID interno para rastreamento |
| `walletId` | string | ID da carteira Asaas |
| `description` | string | Descrição da assinatura |

## Resposta da API

```json
{
  "object": "subscription",
  "id": "sub_VXJBYgP2u0eO",
  "customer": "cus_0T1mdomVMi39",
  "billingType": "PIX",
  "value": 49.90,
  "cycle": "MONTHLY",
  "nextDueDate": "2026-06-09",
  "status": "ACTIVE",
  "paymentLink": "https://asaas.com/c/abc123"
}
```

## Erros Comuns

| Código | Descrição | Solução |
|--------|-----------|---------|
| `invalid_customer` | Cliente não encontrado | Verificar ID do cliente |
| `invalid_wallet` | Carteira inválida | Verificar WalletId |
| `invalid_value` | Valor inválido | Valor deve ser > 0 |