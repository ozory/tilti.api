# SKILL: Implementar Command Handlers Seguindo o Padrão do Projeto

## Quando Usar Esta SKILL
Use esta SKILL quando precisar implementar um novo CommandHandler no projeto. O padrão CQRS do projeto requer que todos os CommandHandlers sigam uma estrutura específica com a interface `ICommandHandler<TCommand, TResponse>`.

## Restrições Importantes
- **NÃO USE** MediatR - é expressamente proibido no projeto
- **NÃO CRIE** interfaces específicas para handlers (ex: `ICreateUserCommandHandler`)
- **USE** a interface genérica `ICommandHandler<TCommand, TResponse>`
- **SEMPRE** retorne `Task<Result<TResponse>>` nos handlers

## Estrutura Obrigatória

### 1. Localização do Arquivo
```
Application/Features/[FeatureName]/Commands/[ActionEntity]/[ActionEntity]CommandHandler.cs
```

### 2. Nomenclatura
- Command: `[Ação][Entidade]Command` (ex: `CreateOrderCommand`)
- CommandHandler: `[Ação][Entidade]CommandHandler` (ex: `CreateOrderCommandHandler`)

### 3. Template de Implementação

```csharp
using Application.Features.[Feature].Contracts;
using Application.Shared.Abstractions;
using Domain.Features.[Feature].Entities;
using Domain.Features.[Feature].Repository;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.[Feature].Commands.[ActionEntity];

/// <summary>
/// Handles creation/update/deletion of [Entity]
/// </summary>
public class [ActionEntity]CommandHandler : ICommandHandler<[ActionEntity]Command, [EntityResponse]>
{
    private readonly I[Feature]Repository _repository;
    private readonly ILogger<[ActionEntity]CommandHandler> _logger;
    private readonly IValidator<[ActionEntity]Command> _validator;
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

    /// <summary>
    /// Executes the command to [action] [entity]
    /// </summary>
    public async Task<Result<[EntityResponse]>> Handle([ActionEntity]Command request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{className}] Executing command: {Command}", _className, request);

        try
        {
            // 1. Validar o comando
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

            // 2. Verificar pré-condições de negócio
            // ...

            // 3. Criar/Atualizar a entidade
            var entity = [Entity].Create(...);

            // 4. Persistir
            var saved[Entity] = await _repository.SaveAsync(entity);

            _logger.LogInformation("[{className}] Command executed successfully: {Id}", _className, saved[Entity].Id);
            return Result.Ok(([EntityResponse])saved[Entity]);
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error executing command: {Request} Error: {Exception}", _className, request, ex);
            return Result.Fail(ex.Message);
        }
    }
}
```

## Checklist de Implementação

- [ ] Herda de `ICommandHandler<[Command], [Response]>`
- [ ] Injeta dependências via construtor
- [ ] Valida o comando no início do Handle
- [ ] Retorna `Result.Fail()` para erros de negócio
- [ ] Retorna `Result.Ok()` para sucesso
- [ ] Loga informações importantes com className
- [ ] Trata exceções sem lançar (catch e log)
- [ ] Assinatura do método: `async Task<Result<[Response]>> Handle(...)`
- [ ] CancellationToken propagado em chamadas async

## Padrão de Tratamento de Erros

```csharp
// ✅ CORRETO
public async Task<Result<OrderResponse>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
{
    try
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

        // ... implementação ...

        return Result.Ok((OrderResponse)saved);
    }
    catch (Exception ex)
    {
        _logger.LogError("[{className}] Error: {Exception}", _className, ex);
        return Result.Fail(ex.Message);
    }
}
```

## Exemplo Real do Projeto

```csharp
// CreateOrderCommandHandler.cs
public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, OrderResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateOrderCommandHandler> _logger;
    private readonly IValidator<CreateOrderCommand> _validator;
    private readonly string className = nameof(CreateOrderCommandHandler);

    public CreateOrderCommandHandler(
        ILogger<CreateOrderCommandHandler> logger,
        IValidator<CreateOrderCommand> validator,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OrderResponse>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{className}] Creating Order: {UserId}", className, request.UserId);

        try
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
                return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

            var order = Order.Create(null, user, request.RequestedTime, request.Addresses, DateTime.Now);
            var savedOrder = await _unitOfWork.OrderRepository.SaveAsync(order);

            _logger.LogInformation("[{className}] Order Created: {OrderId}", className, savedOrder.Id);
            return Result.Ok((OrderResponse)savedOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error: {Request} Error: {Exception}", className, request, ex);
            throw;
        }
    }
}
```

## Dicas Importantes

1. **Sempre use `ICommandHandler<TCommand, TResponse>`** - não invente interfaces específicas
2. **Nunca lance exceções** - capture e retorne `Result.Fail()`
3. **Valide cedo** - validar o comando é a primeira coisa no Handle
4. **Logue tudo** - informação, erro, sucesso
5. **Injete no construtor** - todas as dependências devem ser injetadas
6. **Use CancellationToken** - sempre passe em chamadas async
7. **Retorne Result<T>** - nunca retorne null ou valores simples

## Referências
- Arquivo de instruções: `.github/copilot-instructions.md`
- Exemplo no projeto: `Application/Features/Orders/Commands/Create/CreateOrderCommandHandler.cs`
