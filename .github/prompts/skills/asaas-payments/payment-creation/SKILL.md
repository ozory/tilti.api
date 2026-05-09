# Asaas Payment Creation

**WORKFLOW SKILL** — Criar cobranças únicas via API Asaas. Use para pagamentos avulsos, taxas, ou cobranças que não são recorrentes.

## Quando Usar

- Criar cobrança PIX única
- Criar boleto bancário
- Criar cobrança com cartão
- Gerar link de pagamento para cobrança

## Payload para Cobrança PIX Única

```json
{
  "customer": "cus_XXXXXXXX",
  "billingType": "PIX",
  "value": 99.90,
  "dueDate": "2026-06-16",
  "description": "Taxa de adesão - Plano Pro",
  "externalReference": "payment_internal_id",
  "walletId": "wallet_id_asaas"
}
```

## Payload para Boleto

```json
{
  "customer": "cus_XXXXXXXX",
  "billingType": "BOLETO",
  "value": 199.90,
  "dueDate": "2026-06-16",
  "description": "Mensalidade Junho",
  "externalReference": "payment_internal_id"
}
```

## Implementação CQRS

### Command
```csharp
public record CreatePaymentCommand(
    string CustomerId,
    decimal Value,
    string BillingType,
    DateTime DueDate,
    string Description,
    string ExternalReference,
    string? WalletId = null
) : ICommand<PaymentResponse>;
```

### Handler
```csharp
public class CreatePaymentCommandHandler : ICommandHandler<CreatePaymentCommand, PaymentResponse>
{
    private readonly IAsaasService _asaasService;
    private readonly IPaymentRepository _repository;
    private readonly ILogger<CreatePaymentCommandHandler> _logger;
    private readonly string _className = nameof(CreatePaymentCommandHandler);

    public async Task<Result<PaymentResponse>> Handle(
        CreatePaymentCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("[{className}] Creating payment for customer {CustomerId}", 
                _className, request.CustomerId);

            var asaasRequest = new AsaasPaymentRequest
            {
                Customer = request.CustomerId,
                BillingType = request.BillingType,
                Value = request.Value,
                DueDate = request.DueDate.ToString("yyyy-MM-dd"),
                Description = request.Description,
                ExternalReference = request.ExternalReference,
                WalletId = request.WalletId
            };

            var asaasResponse = await _asaasService.CreatePaymentAsync(asaasRequest);

            var payment = Payment.Create(
                customerId: request.CustomerId,
                value: request.Value,
                billingType: request.BillingType,
                dueDate: request.DueDate,
                asaasPaymentId: asaasResponse.Id,
                asaasPaymentLink: asaasResponse.PaymentLink,
                description: request.Description
            );

            await _repository.SaveAsync(payment);

            return Result.Ok((PaymentResponse)payment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{className}] Error creating payment", _className);
            return Result.Fail($"Failed to create payment: {ex.Message}");
        }
    }
}
```

## Resposta da API

```json
{
  "object": "payment",
  "id": "pay_XXXXXXXX",
  "customer": "cus_XXXXXXXX",
  "billingType": "PIX",
  "value": 99.90,
  "dueDate": "2026-06-16",
  "status": "PENDING",
  "paymentLink": "https://asaas.com/c/abc123"
}
```

## Status de Pagamento

| Status | Descrição |
|--------|-----------|
| `PENDING` | Aguardando pagamento |
| `RECEIVED` | Pagamento recebido |
| `CONFIRMED` | Pagamento confirmado |
| `OVERDUE` | Pagamento vencido |
| `CANCELED` | Pagamento cancelado |

## Campos Obrigatórios

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `customer` | string | ID do cliente no Asaas |
| `value` | number | Valor da cobrança |
| `billingType` | string | Tipo (PIX, BOLETO, CREDIT_CARD) |

## Campos Opcionais Úteis

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `dueDate` | string | Data de vencimento (yyyy-MM-dd) |
| `description` | string | Descrição da cobrança |
| `externalReference` | string | ID interno para rastreamento |
| `walletId` | string | ID da carteira Asaas |