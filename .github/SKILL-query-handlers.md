# SKILL: Implementar Query Handlers Seguindo o Padrão do Projeto

## Quando Usar Esta SKILL
Use esta SKILL quando precisar implementar um novo QueryHandler no projeto. O padrão CQRS do projeto requer que todos os QueryHandlers sigam uma estrutura específica com a interface `IQueryHandler<TQuery, TResponse>`.

## Restrições Importantes
- **NÃO USE** MediatR - é expressamente proibido no projeto
- **NÃO CRIE** interfaces específicas para handlers (ex: `IGetOrderByIdQueryHandler`)
- **USE** a interface genérica `IQueryHandler<TQuery, TResponse>`
- **SEMPRE** retorne `Task<Result<TResponse>>` nos handlers

## Estrutura Obrigatória

### 1. Localização do Arquivo
```
Application/Features/[FeatureName]/Queries/[Action]/[Action]QueryHandler.cs
```

### 2. Nomenclatura
- Query: `[Ação][Entidade]Query` (ex: `GetOrderByIdQuery`, `ListOrdersQuery`)
- QueryHandler: `[Ação][Entidade]QueryHandler` (ex: `GetOrderByIdQueryHandler`, `ListOrdersQueryHandler`)

### 3. Template de Implementação

```csharp
using Application.Features.[Feature].Contracts;
using Application.Shared.Abstractions;
using Domain.Features.[Feature].Repository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Features.[Feature].Queries.[Action];

/// <summary>
/// Handles queries to retrieve [Entity]
/// </summary>
public class [Action]QueryHandler : IQueryHandler<[Action]Query, [EntityResponse]>
{
    private readonly I[Feature]Repository _repository;
    private readonly ILogger<[Action]QueryHandler> _logger;
    private readonly string _className = nameof([Action]QueryHandler);

    public [Action]QueryHandler(
        ILogger<[Action]QueryHandler> logger,
        I[Feature]Repository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    /// <summary>
    /// Executes the query to retrieve [Entity]
    /// </summary>
    public async Task<Result<[EntityResponse]>> Handle([Action]Query request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Buscar dados
            var entity = await _repository.GetByIdAsync(request.Id);

            // 2. Validar resultado
            if (entity == null)
                return Result.Fail("[Entity] not found");

            _logger.LogInformation("[{className}] Query executed successfully: {Id}", _className, request.Id);
            return Result.Ok(([EntityResponse])entity);
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error executing query: {Request} Error: {Exception}", _className, request, ex);
            return Result.Fail(ex.Message);
        }
    }
}
```

## Checklist de Implementação

- [ ] Herda de `IQueryHandler<[Query], [Response]>`
- [ ] Injeta repositório(s) necessário(s) via construtor
- [ ] Não faz validação complexa (queries devem ser simples)
- [ ] Valida resultado nulo/vazio
- [ ] Retorna `Result.Fail()` para erros de negócio
- [ ] Retorna `Result.Ok()` para sucesso
- [ ] Loga informações com className
- [ ] Trata exceções sem lançar
- [ ] Assinatura do método: `async Task<Result<[Response]>> Handle(...)`
- [ ] CancellationToken propagado em chamadas async

## Padrão de Tratamento de Erros

```csharp
// ✅ CORRETO
public async Task<Result<OrderResponse>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
{
    try
    {
        var order = await _repository.GetByIdAsync(request.OrderId);
        
        if (order == null)
            return Result.Fail("Order not found");

        return Result.Ok((OrderResponse)order);
    }
    catch (Exception ex)
    {
        _logger.LogError("[{className}] Error: {Exception}", _className, ex);
        return Result.Fail(ex.Message);
    }
}
```

## Diferenças entre Command e Query Handlers

| Aspecto | Command Handler | Query Handler |
|--------|----------------|---------------|
| Interface | `ICommandHandler<>` | `IQueryHandler<>` |
| Validação | Completa com `IValidator<>` | Mínima |
| Efeitos Colaterais | Sim (persiste dados) | Não (apenas lê) |
| Transação | Geralmente sim | Não |
| Logging | Mais detalhado | Básico |
| Tratamento Erro | Mais robusto | Simples |

## Exemplos Reais do Projeto

### Query Simples - GetById
```csharp
public class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIdQuery, OrderResponse>
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<GetOrderByIdQueryHandler> _logger;
    private readonly string _className = nameof(GetOrderByIdQueryHandler);

    public GetOrderByIdQueryHandler(
        ILogger<GetOrderByIdQueryHandler> logger,
        IOrderRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<Result<OrderResponse>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _repository.GetByIdAsync(request.OrderId);
            if (order == null)
                return Result.Fail("Order not found");

            return Result.Ok((OrderResponse)order);
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error: {Exception}", _className, ex);
            return Result.Fail(ex.Message);
        }
    }
}
```

### Query com Filtros - ListOrders
```csharp
public class ListOrdersQueryHandler : IQueryHandler<ListOrdersQuery, IReadOnlyList<OrderResponse>>
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<ListOrdersQueryHandler> _logger;
    private readonly string _className = nameof(ListOrdersQueryHandler);

    public ListOrdersQueryHandler(
        ILogger<ListOrdersQueryHandler> logger,
        IOrderRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<OrderResponse>>> Handle(ListOrdersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var orders = await _repository.GetAllAsync();
            var response = orders.Select(o => (OrderResponse)o).ToList();

            return Result.Ok(response as IReadOnlyList<OrderResponse>);
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error: {Exception}", _className, ex);
            return Result.Fail(ex.Message);
        }
    }
}
```

## Boas Práticas

1. **Queries devem ser simples** - apenas leitura, sem validação complexa
2. **Não injete validators** - queries não precisam validar
3. **Use apenas repositórios** - não dependa de services complexos
4. **Retorne dados projetados** - use DTOs/Responses, não entidades
5. **Trate nulls gracefully** - retorne `Result.Fail()` para dados não encontrados
6. **Logue minimamente** - não poluí logs com queries triviais
7. **Reutilize methods de repo** - não reimplemente lógica de filtro

## Referências
- Arquivo de instruções: `.github/copilot-instructions.md`
- SKILL de Commands: `.github/SKILL-command-handlers.md`
