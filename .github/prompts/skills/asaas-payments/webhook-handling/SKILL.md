# Asaas Webhook Handling

**WORKFLOW SKILL** — Processar webhooks da Asaas para atualizar status de assinaturas e pagamentos. Use para implementar handlers de webhook que atualizam o sistema local.

## Quando Usar

- Processar confirmação de pagamento
- Ativar assinatura após pagamento
- Inativar assinatura expirada
- Cancelar assinatura
- Atualizar data de vencimento

## Eventos Principais

| Evento | Ação | Handler |
|--------|------|---------|
| `PAYMENT_RECEIVED` | Pagamento recebido | `ActivateSubscriptionCommand` |
| `PAYMENT_CONFIRMED` | Pagamento confirmado | `ConfirmPaymentCommand` |
| `PAYMENT_EXPIRED` | Pagamento expirado | `InactivateSubscriptionCommand` |
| `PAYMENT_CANCELED` | Pagamento cancelado | `CancelSubscriptionCommand` |

## Payload do Webhook

```json
{
  "event": "PAYMENT_RECEIVED",
  "payment": {
    "object": "payment",
    "id": "pay_XXXXXXXX",
    "customer": "cus_XXXXXXXX",
    "subscription": "sub_XXXXXXXX",
    "value": 49.90,
    "status": "RECEIVED",
    "dueDate": "2026-06-09",
    "paymentDate": "2026-06-09",
    "externalReference": "sub_internal_id"
  }
}
```

## Implementação do Endpoint

### WebhookEndpoint.cs
```csharp
[HttpPost("/webhooks/asaas")]
public async Task<IResult> HandleAsaasWebhook(
    [FromBody] AsaasWebhookRequest request,
    [FromServices] ICommandDispatcher dispatcher)
{
    try
    {
        switch (request.Event)
        {
            case "PAYMENT_RECEIVED":
            case "PAYMENT_CONFIRMED":
                await dispatcher.Dispatch(new ActivateSubscriptionCommand(
                    request.Payment.Subscription,
                    request.Payment.Id,
                    request.Payment.PaymentDate
                ));
                break;

            case "PAYMENT_EXPIRED":
            case "PAYMENT_CANCELED":
                await dispatcher.Dispatch(new InactivateSubscriptionCommand(
                    request.Payment.Subscription
                ));
                break;
        }

        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
}
```

## Handler para Ativação

```csharp
public class ActivateSubscriptionCommandHandler : ICommandHandler<ActivateSubscriptionCommand>
{
    private readonly ISubscriptionRepository _repository;
    private readonly ILogger<ActivateSubscriptionCommandHandler> _logger;
    private readonly string _className = nameof(ActivateSubscriptionCommandHandler);

    public async Task<Result> Handle(
        ActivateSubscriptionCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var subscription = await _repository.GetByAsaasSubscriptionIdAsync(request.SubscriptionId);
            
            if (subscription == null)
                return Result.Fail("Subscription not found");

            subscription.MarkAsPaid(request.PaymentId, request.PaidAt);
            await _repository.UpdateAsync(subscription);

            _logger.LogInformation("[{className}] Subscription {Id} activated", 
                _className, subscription.Id);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{className}] Error activating subscription", _className);
            return Result.Fail(ex.Message);
        }
    }
}
```

## Busca por ExternalReference

```csharp
// No repositório
public async Task<Subscription?> GetByExternalReferenceAsync(string externalReference)
{
    return await _dbContext.Subscriptions
        .FirstOrDefaultAsync(s => s.ExternalReference == externalReference);
}
```

## Validação de Segurança

```csharp
[HttpPost("/webhooks/asaas")]
public async Task<IResult> HandleAsaasWebhook(
    [FromHeader(Name = "asaas-signature")] string signature,
    [FromBody] AsaasWebhookRequest request)
{
    // Validar assinatura do webhook
    if (!ValidateSignature(signature, request))
        return Results.Unauthorized();

    // Processar evento
    // ...
}
```

## Fluxo Completo

```
1. Asaas → Webhook → /webhooks/asaas
2. Extrair evento (PAYMENT_RECEIVED)
3. Extrair externalReference ou subscription ID
4. Dispatch command baseado no evento
5. Atualizar subscription local
6. Retornar 200 OK
```

## Testes no Sandbox

1. Criar assinatura via API
2. No painel Asaas Sandbox, localizar a cobrança
3. Marcar como "Recebido" manualmente
4. Verificar se webhook foi disparado
5. Confirmar atualização no banco local