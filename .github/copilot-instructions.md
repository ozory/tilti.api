# GitHub Copilot Instructions

## Padrões de Desenvolvimento

### Estrutura de Pastas
- Todos os Commands devem ser criados em: `Application/Features/[FeatureName]/Commands`
- Todos os Queries devem ser criados em: `Application/Features/[FeatureName]/Queries`
- Todos os Eventos devem ser criados em: `Application/Features/[FeatureName]/Events`

### Padrão de Nomenclatura
- Commands: `[Ação][Entidade]Command` (ex: `CreateUserCommand`, `UpdateOrderCommand`)
- Command Handlers: `[Ação][Entidade]CommandHandler` (ex: `CreateUserCommandHandler`)
- Queries: `[Ação][Entidade]Query` (ex: `GetUserByIdQuery`, `ListOrdersQuery`)
- Query Handlers: `[Ação][Entidade]QueryHandler` (ex: `GetUserByIdQueryHandler`)
- Eventos: `[Nome]Event` (ex: `OrderCreatedEvent`, `UserUpdatedEvent`)
- Event Handlers: `[Nome]EventHandler` (ex: `OrderCreatedEventHandler`)

### Regras de Implementação
- **PROIBIDO USAR** MediatR ou qualquer dependência relacionada ao MediatR
- **OBRIGATÓRIO USAR** `ICommandHandler<TCommand, TResponse>` interface do projeto
- **NÃO CRIAR** interfaces específicas para cada handler (ex: `ICreateUserCommandHandler`)
- Seguir o padrão de Injeção de Dependência existente no projeto
- Sempre retornar `Task<Result<TResponse>>` nos handlers de Command
- Sempre retornar `Task<Result<TResponse>>` nos handlers de Query

### Implementação CQRS Correta

#### Padrão de Command Handler
Os CommandHandlers devem implementar a interface `ICommandHandler<TCommand, TResponse>`:

```csharp
using Application.Features.Orders.Contracts;
using Application.Shared.Abstractions;
using Domain.Features.Orders.Entities;
using Domain.Features.Orders.Repository;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, OrderResponse>
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<CreateOrderCommandHandler> _logger;
    private readonly IValidator<CreateOrderCommand> _validator;
    private readonly string _className = nameof(CreateOrderCommandHandler);

    public CreateOrderCommandHandler(
        ILogger<CreateOrderCommandHandler> logger,
        IValidator<CreateOrderCommand> validator,
        IOrderRepository repository)
    {
        _logger = logger;
        _validator = validator;
        _repository = repository;
    }

    public async Task<Result<OrderResponse>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{className}] Creating Order: {OrderId}", _className, request.Id);

        try
        {
            // Validar command
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

            // Executar lógica de negócios
            var order = Order.Create(null, request.UserId, request.Amount, DateTime.Now);

            // Persistir dados
            var savedOrder = await _repository.SaveAsync(order);

            _logger.LogInformation("[{className}] Order created successfully: {OrderId}", _className, savedOrder.Id);
            return Result.Ok((OrderResponse)savedOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error creating Order: {Request} Error: {Exception}", _className, request, ex);
            return Result.Fail(ex.Message);
        }
    }
}
```

#### Padrão de Query Handler
Os QueryHandlers devem implementar a interface `IQueryHandler<TQuery, TResponse>`:

```csharp
using Application.Features.Orders.Contracts;
using Application.Shared.Abstractions;
using Domain.Features.Orders.Repository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Queries.GetOrderById;

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
            _logger.LogError("[{className}] Error getting Order: {Exception}", _className, ex);
            return Result.Fail(ex.Message);
        }
    }
}
```

### Padrões de Código
- Sempre usar `ICommandHandler<TCommand, TResponse>` para Commands
- Sempre usar `IQueryHandler<TQuery, TResponse>` para Queries
- Sempre envolver resultado em `Result<T>` do FluentResults
- Sempre logar informações importantes com className
- Sempre tratar exceções e retornar `Result.Fail()` ao invés de lançar exceções
- Seguir o mesmo estilo e padrão dos arquivos existentes
- Manter consistência com as implementações já existentes
- Utilizar injeção de dependência via construtor
- Seguir as convenções de código do projeto (naming, comments, etc)