# Asaas Subscription Cancellation

**WORKFLOW SKILL** — Cancelar assinaturas via API Asaas. Use para implementar fluxos de cancelamento de assinatura por usuário ou administrador.

## Quando Usar

- Cancelar assinatura de motorista
- Cancelar assinatura de passageiro
- Processar cancelamento via webhook
- Remover assinatura expirada

## Cancelamento via API

### DELETE /v3/subscriptions/{id}

```csharp
public class CancelSubscriptionCommandHandler : ICommandHandler<CancelSubscriptionCommand>
{
    private readonly IAsaasService _asaasService;
    private readonly ISubscriptionRepository _repository;
    private readonly ILogger<CancelSubscriptionCommandHandler> _logger;
    private readonly string _className = nameof(CancelSubscriptionCommandHandler);

    public async Task<Result> Handle(
        CancelSubscriptionCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var subscription = await _repository.GetByIdAsync(request.SubscriptionId);
            
            if (subscription == null)
                return Result.Fail("Subscription not found");

            // Cancelar no Asaas
            await _asaasService.CancelSubscriptionAsync(subscription.AsaasSubscriptionId);

            // Atualizar localmente
            subscription.Cancel();
            await _repository.UpdateAsync(subscription);

            _logger.LogInformation("[{className}] Subscription {Id} cancelled", 
                _className, subscription.Id);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{className}] Error cancelling subscription", _className);
            return Result.Fail(ex.Message);
        }
    }
}
```

## Cancelamento via Webhook

```csharp
// No WebhookEndpoint.cs
case "PAYMENT_EXPIRED":
case "PAYMENT_CANCELED":
    await dispatcher.Dispatch(new CancelSubscriptionCommand(
        request.Payment.Subscription
    ));
    break;
```

## Command

```csharp
public record CancelSubscriptionCommand(string SubscriptionId) : ICommand;
```

## Entidade Subscription

```csharp
public class Subscription
{
    public void Cancel()
    {
        Status = SubscriptionStatus.Inactive;
        CancelledAt = DateTime.UtcNow;
    }

    public void Inactivate()
    {
        Status = SubscriptionStatus.Inactive;
    }
}
```

## Resposta da API

```json
{
  "deleted": true
}
```

## Fluxo de Cancelamento

```
1. Usuário solicita cancelamento
2. Buscar subscription local pelo ID
3. Cancelar no Asaas via DELETE /v3/subscriptions/{id}
4. Atualizar subscription local com status Inactive
5. Retornar sucesso
```

## Cancelamento com Motivo

```csharp
public record CancelSubscriptionCommand(
    string SubscriptionId, 
    string? Reason = null
) : ICommand;
```

## Tratamento de Erros

| Código | Descrição | Solução |
|--------|-----------|---------|
| `subscription_not_found` | Assinatura não existe | Verificar ID |
| `subscription_already_cancelled` | Já cancelada | Ignorar erro |
| `invalid_subscription` | ID inválido | Validar formato |