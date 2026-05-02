# Cursor Rules - Padrões CQRS do Projeto

Este arquivo contém regras customizadas para o Cursor seguir ao implementar código no projeto Tilt.

## Regra 1: Padrão CQRS - Proibição de MediatR

**Quando aplicar:** Sempre que for implementar Command Handlers, Query Handlers ou Event Handlers

**Regra:**
```
❌ NUNCA use MediatR
❌ NUNCA crie interfaces específicas para handlers (ex: ICreateOrderCommandHandler)
✅ SEMPRE use ICommandHandler<TCommand, TResponse>
✅ SEMPRE use IQueryHandler<TQuery, TResponse>
```

**Consequência:** Se o Cursor sugerir usar MediatR, rejeite e corrija para o padrão correto.

## Regra 2: Implementação de Command Handlers

**Padrão obrigatório:**

```csharp
public class [ActionEntity]CommandHandler : ICommandHandler<[ActionEntity]Command, [EntityResponse]>
{
    private readonly ILogger<[ActionEntity]CommandHandler> _logger;
    private readonly IValidator<[ActionEntity]Command> _validator;
    private readonly I[Feature]Repository _repository;
    private readonly string _className = nameof([ActionEntity]CommandHandler);

    public [ActionEntity]CommandHandler(
        ILogger<[ActionEntity]CommandHandler> logger,
        IValidator<[ActionEntity]Command> validator,
        I[Feature]Repository repository)
    {
        _logger = logger;
        _validator = validator;
        _repository = repository;
    }

    public async Task<Result<[EntityResponse]>> Handle([ActionEntity]Command request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{className}] Executing command", _className);
        
        try
        {
            // 1. Validar
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

            // 2. Implementar lógica
            // ...

            // 3. Persistir
            var saved = await _repository.SaveAsync(entity);

            return Result.Ok(([EntityResponse])saved);
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error: {Exception}", _className, ex);
            return Result.Fail(ex.Message);
        }
    }
}
```

**Verificação:**
- [ ] Implementa `ICommandHandler<TCommand, TResponse>`
- [ ] Retorna `Task<Result<TResponse>>`
- [ ] Valida comando no início
- [ ] Captura exceções e retorna `Result.Fail()`
- [ ] Loga com `_className`
- [ ] Injeta dependências via construtor

## Regra 3: Implementação de Query Handlers

**Padrão obrigatório:**

```csharp
public class [Action]QueryHandler : IQueryHandler<[Action]Query, [EntityResponse]>
{
    private readonly ILogger<[Action]QueryHandler> _logger;
    private readonly I[Feature]Repository _repository;
    private readonly string _className = nameof([Action]QueryHandler);

    public [Action]QueryHandler(
        ILogger<[Action]QueryHandler> logger,
        I[Feature]Repository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<Result<[EntityResponse]>> Handle([Action]Query request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            
            if (entity == null)
                return Result.Fail("Entity not found");

            return Result.Ok(([EntityResponse])entity);
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error: {Exception}", _className, ex);
            return Result.Fail(ex.Message);
        }
    }
}
```

**Verificação:**
- [ ] Implementa `IQueryHandler<TQuery, TResponse>`
- [ ] Retorna `Task<Result<TResponse>>`
- [ ] Sem validação complexa
- [ ] Trata nulls
- [ ] Captura exceções

## Regra 4: Nomenclatura Obrigatória

| Tipo | Padrão | Exemplo |
|------|--------|---------|
| Command | `[Ação][Entidade]Command` | `CreateOrderCommand` |
| CommandHandler | `[Ação][Entidade]CommandHandler` | `CreateOrderCommandHandler` |
| Query | `[Ação][Entidade]Query` | `GetOrderByIdQuery` |
| QueryHandler | `[Ação][Entidade]QueryHandler` | `GetOrderByIdQueryHandler` |
| Interface de Repo | `I[Entidade]Repository` | `IOrderRepository` |

## Regra 5: Estrutura de Pastas

```
Application/
  Features/
    [FeatureName]/
      Commands/
        [ActionEntity]/
          [ActionEntity]Command.cs
          [ActionEntity]CommandHandler.cs
          [ActionEntity]Validator.cs
      Queries/
        [Action]/
          [Action]Query.cs
          [Action]QueryHandler.cs
      Contracts/
        [EntityResponse].cs
```

## Regra 6: Uso de FluentResults

**Sempre retorne Result<T>:**

```csharp
// ✅ CORRETO
return Result.Ok(value);
return Result.Fail("error message");
return Result.Fail(errors.Select(x => x.ErrorMessage));

// ❌ ERRADO
return value;
return null;
throw new Exception("error");
```

## Regra 7: Logging Padrão

**Sempre use className:**

```csharp
private readonly string _className = nameof(CreateOrderCommandHandler);

// Use em logs
_logger.LogInformation("[{className}] Action performed", _className);
_logger.LogError("[{className}] Error occurred: {Exception}", _className, ex);
```

## Regra 8: Injeção de Dependência

**Sempre via construtor:**

```csharp
// ✅ CORRETO
public CreateOrderCommandHandler(
    ILogger<CreateOrderCommandHandler> logger,
    IValidator<CreateOrderCommand> validator,
    IOrderRepository repository)
{
    _logger = logger;
    _validator = validator;
    _repository = repository;
}

// ❌ ERRADO
public CreateOrderCommandHandler()
{
    _logger = ServiceLocator.GetLogger();  // Service Locator
}
```

## Integração com o Projeto

**Para usar estas regras no Cursor:**

1. Adicione este arquivo ao `.cursor/rules.md` (ou pasta `.cursorrules`)
2. O Cursor lerá automaticamente e aplicará as regras
3. Configure em `Cursor Settings > Rules` se necessário

## Documentação Relacionada

- **Instruções Principal:** `/.github/copilot-instructions.md`
- **SKILL Commands:** `/.github/SKILL-command-handlers.md`
- **SKILL Queries:** `/.github/SKILL-query-handlers.md`
- **Exemplo Real:** `/Application/Features/Orders/Commands/Create/CreateOrderCommandHandler.cs`

## Checklist para Code Review

Ao revisar código, verifique:

- [ ] Usa `ICommandHandler<>` ou `IQueryHandler<>` (não MediatR)
- [ ] Retorna `Task<Result<T>>`
- [ ] Valida entrada (commands)
- [ ] Trata exceções sem lançar
- [ ] Loga com className
- [ ] Injeta dependências via construtor
- [ ] Segue nomenclatura `[Ação][Entidade]`
- [ ] Localizado na pasta correta
