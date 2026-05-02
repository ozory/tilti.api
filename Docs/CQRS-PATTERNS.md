# Guia de Padrões CQRS - Projeto Tilt

Este documento explica como os padrões CQRS estão documentados e configurados no projeto, para guiar o GitHub Copilot, Cursor e outros agentes de IA na implementação correta.

## 📁 Arquivos de Documentação

### 1. **`.github/copilot-instructions.md`**
**Propósito:** Instruções globais para GitHub Copilot
**Conteúdo:**
- Estrutura de pastas do projeto
- Padrão de nomenclatura
- Regras de implementação
- Exemplos de Code (Commands e Queries)

**Quando é usado:**
- GitHub Copilot lê automaticamente este arquivo
- Aplicável a qualquer arquivo aberto no projeto

### 2. **`.github/SKILL-command-handlers.md`**
**Propósito:** SKILL específica para implementação de Command Handlers
**Conteúdo:**
- Template completo de implementação
- Checklist de verificação
- Padrão de tratamento de erros
- Exemplos reais do projeto
- Dicas importantes

**Como usar:**
```
# No GitHub Copilot, mencione:
@github /help SKILL-command-handlers
# ou faça uma pergunta relacionada a command handler
```

### 3. **`.github/SKILL-query-handlers.md`**
**Propósito:** SKILL específica para implementação de Query Handlers
**Conteúdo:**
- Template de implementação para queries
- Diferenças entre Commands e Queries
- Boas práticas específicas
- Exemplos de queries simples e complexas

**Como usar:**
```
# No GitHub Copilot, mencione:
@github /help SKILL-query-handlers
# ou faça uma pergunta relacionada a query handler
```

### 4. **`.cursor/rules.md`**
**Propósito:** Regras customizadas para Cursor AI
**Conteúdo:**
- Regras de implementação (8 regras principais)
- Padrões obrigatórios em código
- Verificação e checklist
- Integração com projeto

**Como usar:**
- Cursor lê automaticamente `/.cursor/rules.md`
- Não requer configuração adicional
- Aplica-se a qualquer novo arquivo criado

## 🎯 Fluxo de Trabalho

### Implementar um Novo Command Handler

1. **Abra qualquer arquivo do projeto**
   - GitHub Copilot carrega automaticamente `copilot-instructions.md`
   - Cursor carrega automaticamente `.cursor/rules.md`

2. **Crie o novo CommandHandler**
   - Localize em: `Application/Features/[Feature]/Commands/[Action]/`
   - Nomeie como: `[Action][Entity]CommandHandler.cs`

3. **Peça ao Copilot/Cursor:**
   ```
   "Implemente um CommandHandler para CreateOrder seguindo o padrão do projeto"
   ```

4. **O agente deve:**
   - Usar `ICommandHandler<CreateOrderCommand, OrderResponse>`
   - Validar o comando
   - Retornar `Result<T>` do FluentResults
   - Logar com className
   - Tratar exceções

### Implementar uma Nova Query

1. **Crie o novo QueryHandler**
   - Localize em: `Application/Features/[Feature]/Queries/[Action]/`
   - Nomeie como: `[Action][Entity]QueryHandler.cs`

2. **Peça ao Copilot/Cursor:**
   ```
   "Implemente um QueryHandler para GetOrderById seguindo o padrão do projeto"
   ```

3. **O agente deve:**
   - Usar `IQueryHandler<GetOrderByIdQuery, OrderResponse>`
   - Buscar dados no repositório
   - Validar resultado nulo
   - Retornar `Result<T>`

## ✅ Padrão Obrigatório

### Command Handler
```csharp
public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, OrderResponse>
{
    private readonly ILogger<CreateOrderCommandHandler> _logger;
    private readonly IValidator<CreateOrderCommand> _validator;
    private readonly IOrderRepository _repository;
    private readonly string _className = nameof(CreateOrderCommandHandler);

    // ... implementação seguindo o padrão ...

    public async Task<Result<OrderResponse>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Validar → Processar → Retornar Result<T>
    }
}
```

### Query Handler
```csharp
public class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIdQuery, OrderResponse>
{
    private readonly ILogger<GetOrderByIdQueryHandler> _logger;
    private readonly IOrderRepository _repository;
    private readonly string _className = nameof(GetOrderByIdQueryHandler);

    // ... implementação seguindo o padrão ...

    public async Task<Result<OrderResponse>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        // Buscar → Validar → Retornar Result<T>
    }
}
```

## 🚫 Proibições

| Proibido | Razão |
|----------|-------|
| ❌ MediatR | Projeto usa padrão customizado |
| ❌ `ICreateOrderCommandHandler` | Use `ICommandHandler<>` genérico |
| ❌ Lançar exceções | Retorne `Result.Fail()` |
| ❌ Service Locator | Injete via construtor |
| ❌ Validator em Query Handler | Queries são simples |

## 📚 Referência Rápida

### Nomenclatura
- **Command:** `CreateOrderCommand`, `UpdateOrderCommand`, `DeleteOrderCommand`
- **Handler:** `CreateOrderCommandHandler`, `UpdateOrderCommandHandler`
- **Query:** `GetOrderByIdQuery`, `ListOrdersQuery`
- **Handler:** `GetOrderByIdQueryHandler`, `ListOrdersQueryHandler`

### Estrutura de Pastas
```
Application/Features/Orders/
├── Commands/
│   ├── CreateOrder/
│   │   ├── CreateOrderCommand.cs
│   │   ├── CreateOrderCommandHandler.cs
│   │   └── CreateOrderValidator.cs
│   └── UpdateOrder/
│       ├── UpdateOrderCommand.cs
│       ├── UpdateOrderCommandHandler.cs
│       └── UpdateOrderValidator.cs
├── Queries/
│   ├── GetOrderById/
│   │   ├── GetOrderByIdQuery.cs
│   │   └── GetOrderByIdQueryHandler.cs
│   └── ListOrders/
│       ├── ListOrdersQuery.cs
│       └── ListOrdersQueryHandler.cs
└── Contracts/
    └── OrderResponse.cs
```

### Return Pattern
```csharp
// Sucesso
return Result.Ok((OrderResponse)order);

// Erro de validação
return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

// Erro de negócio
return Result.Fail("Order not found");

// Erro de exceção
return Result.Fail(ex.Message);
```

## 🔍 Verificação de Qualidade

Antes de fazer commit, verifique:

- [ ] Usa `ICommandHandler<>` ou `IQueryHandler<>`
- [ ] Método `Handle()` retorna `Task<Result<T>>`
- [ ] Nomeado seguindo padrão `[Ação][Entidade]`
- [ ] Localizado em pasta correta
- [ ] Injeta dependências via construtor
- [ ] Loga com className
- [ ] Trata exceções sem lançar

## 💡 Dicas para Usar com IA

### GitHub Copilot
```
# Peça explicitamente para seguir o padrão
"Implemente CreateOrderCommandHandler seguindo o padrão CQRS 
do projeto com ICommandHandler<CreateOrderCommand, OrderResponse>"

# Reference a exemplo existente
"Use como referência CreateUserCommandHandler em 
Application/Features/Users/Commands/"

# Peça para incluir logging
"Implemente com logging usando _className e _logger"
```

### Cursor
```
# As regras são aplicadas automaticamente
# Mas você pode mencionar explicitamente:
"Siga as regras CQRS do projeto em .cursor/rules.md"

# Use Ctrl+Shift+/ para ver as regras ativas
```

## 📖 Documentação Completa

Para documentação detalhada:
- **Padrão de Commands:** Veja `.github/SKILL-command-handlers.md`
- **Padrão de Queries:** Veja `.github/SKILL-query-handlers.md`
- **Regras de Cursor:** Veja `.cursor/rules.md`
- **Instruções gerais:** Veja `.github/copilot-instructions.md`

## 🆘 Solução de Problemas

### Copilot sugere usar MediatR
- ❌ Rejeite a sugestão
- ✅ Corrija para `ICommandHandler<>`
- 📝 Mencione que MediatR é proibido

### Copilot cria interface específica
- ❌ `ICreateOrderCommandHandler`
- ✅ Use `ICommandHandler<CreateOrderCommand, OrderResponse>`

### Handler não segue o padrão
- Copie o template de `.github/SKILL-command-handlers.md`
- Verifique exemplo em `/Application/Features/Orders/Commands/Create/`

### Cursor não aplica as regras
- Verifique se `.cursor/rules.md` existe
- Reinicie o Cursor
- Configure em `Cursor Settings > Rules`

---

**Última atualização:** Maio 2026  
**Responsável:** Arquitetura CQRS do Projeto Tilt
