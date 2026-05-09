# Asaas Payments Integration

**WORKFLOW SKILL** — Integração com a API Asaas para pagamentos e assinaturas. Use para criar, consultar, atualizar ou remover cobranças e assinaturas via API Asaas.

## Quando Usar Esta Skill

- Criar novas cobranças (PIX, BOLETO, CARTÃO)
- Criar assinaturas recorrentes
- Consultar status de pagamentos
- Atualizar assinaturas existentes
- Processar webhooks da Asaas
- Cancelar assinaturas ou pagamentos

## Endpoints Principais

### Pagamentos (Payments)
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `POST` | `/v3/payments` | Criar nova cobrança |
| `GET` | `/v3/payments` | Listar cobranças |
| `GET` | `/v3/payments/{id}` | Obter cobrança específica |
| `PUT` | `/v3/payments/{id}` | Atualizar cobrança |
| `DELETE` | `/v3/payments/{id}` | Remover cobrança |

### Assinaturas (Subscriptions)
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `POST` | `/v3/subscriptions` | Criar assinatura |
| `GET` | `/v3/subscriptions` | Listar assinaturas |
| `GET` | `/v3/subscriptions/{id}` | Obter assinatura |
| `PUT` | `/v3/subscriptions/{id}` | Atualizar assinatura |
| `DELETE` | `/v3/subscriptions/{id}` | Cancelar assinatura |
| `GET` | `/v3/subscriptions/{id}/payments` | Listar pagamentos da assinatura |

## Tipos de Cobrança (billingType)

- `PIX` - Pagamento via PIX
- `BOLETO` - Boleto bancário
- `CREDIT_CARD` - Cartão de crédito
- `DEBIT_CARD` - Cartão de débito
- `TRANSFER` - Transferência bancária
- `DEPOSIT` - Depósito bancário

## Ciclos de Assinatura (cycle)

- `WEEKLY` - Semanal
- `BIWEEKLY` - Quinzenal
- `MONTHLY` - Mensal
- `BIMONTHLY` - Bimestral
- `QUARTERLY` - Trimestral
- `SEMIANNUALLY` - Semestral
- `YEARLY` - Anual

## Status de Assinatura

- `ACTIVE` - Ativa
- `INACTIVE` - Inativa
- `EXPIRED` - Expirada

## Padrão de Implementação CQRS

### Estrutura de Pastas
```
Application/Features/Subscriptions/
├── Commands/
│   ├── CreateSubscription/
│   │   ├── CreateSubscriptionCommand.cs
│   │   ├── CreateSubscriptionCommandHandler.cs
│   │   └── CreateSubscriptionCommandValidator.cs
│   └── CancelSubscription/
├── Queries/
│   └── GetSubscriptionById/
└── Contracts/
    └── SubscriptionResponse.cs
```

### Handler Pattern
```csharp
public class CreateSubscriptionCommandHandler : ICommandHandler<CreateSubscriptionCommand, SubscriptionResponse>
{
    private readonly ISubscriptionRepository _repository;
    private readonly IAsaasService _asaasService;
    private readonly ILogger<CreateSubscriptionCommandHandler> _logger;
    private readonly string _className = nameof(CreateSubscriptionCommandHandler);

    public async Task<Result<SubscriptionResponse>> Handle(
        CreateSubscriptionCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar
            // 2. Criar no Asaas
            // 3. Salvar localmente
            // 4. Retornar resultado
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error: {Error}", _className, ex.Message);
            return Result.Fail(ex.Message);
        }
    }
}
```

## Webhooks Importantes

| Evento | Ação |
|--------|------|
| `PAYMENT_RECEIVED` | Ativar assinatura |
| `PAYMENT_CONFIRMED` | Confirmar pagamento |
| `PAYMENT_EXPIRED` | Inativar assinatura |
| `PAYMENT_CANCELED` | Cancelar assinatura |

## Configuração (appsettings.json)
```json
{
  "Asaas": {
    "BaseUrl": "https://api-sandbox.asaas.com",
    "AccessToken": "SUA_API_KEY",
    "WalletId": "ID_DA_CARTEIRA"
  }
}
```

## Referências

- [Asaas API Docs](https://asaas.com/docs/api)
- [Subscription Creation](.github/prompts/skills/asaas-payments/subscription-creation/SKILL.md)
- [Webhook Handling](.github/prompts/skills/asaas-payments/webhook-handling/SKILL.md)