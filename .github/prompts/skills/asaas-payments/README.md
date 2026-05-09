# Asaas Payments Skills

Skills especializadas para integração com a API Asaas de pagamentos e assinaturas.

## Skills Disponíveis

| Skill | Descrição | Quando Usar |
|-------|-----------|-------------|
| [Subscription Creation](subscription-creation/SKILL.md) | Criar assinaturas recorrentes | Nova assinatura PIX, mensal, anual |
| [Payment Creation](payment-creation/SKILL.md) | Criar cobranças únicas | Pagamento avulso, taxa única |
| [Webhook Handling](webhook-handling/SKILL.md) | Processar webhooks da Asaas | Atualizar status após pagamento |
| [Subscription Cancellation](subscription-cancellation/SKILL.md) | Cancelar assinaturas | Cancelamento por usuário/admin |

## Quick Start

### Criar Assinatura PIX Mensal
```csharp
var command = new CreateSubscriptionCommand(
    CustomerId: "cus_XXXXXXXX",
    Value: 49.90m,
    Cycle: "MONTHLY",
    NextDueDate: DateTime.Today.AddDays(30),
    Description: "Assinatura Motorista - Plano Pro",
    ExternalReference: "sub_123",
    WalletId: "wallet_id",
    BillingType: "PIX"
);
```

### Processar Webhook de Pagamento
```csharp
switch (webhook.Event)
{
    case "PAYMENT_RECEIVED":
        await dispatcher.Dispatch(new ActivateSubscriptionCommand(
            webhook.Payment.Subscription,
            webhook.Payment.Id,
            webhook.Payment.PaymentDate
        ));
        break;
}
```

## Estrutura de Pastas

```
.github/prompts/skills/asaas-payments/
├── SKILL.md                    # Visão geral
├── README.md                   # Este arquivo
├── subscription-creation/      # Criar assinaturas
│   └── SKILL.md
├── payment-creation/           # Criar cobranças
│   └── SKILL.md
├── webhook-handling/           # Processar webhooks
│   └── SKILL.md
└── subscription-cancellation/  # Cancelar assinaturas
    └── SKILL.md
```

## Padrões do Projeto

- **Commands**: `CreateSubscriptionCommand`, `CancelSubscriptionCommand`
- **Handlers**: `CreateSubscriptionCommandHandler`, `CancelSubscriptionCommandHandler`
- **Interfaces**: `ICommandHandler<TCommand, TResponse>`, `IQueryHandler<TQuery, TResponse>`
- **Retorno**: Sempre `Task<Result<TResponse>>` ou `Task<Result>`
- **Sem MediatR** - usar `ICommandHandler` do projeto