using Application.Features.Orders.Contracts;
using Application.Features.Users.Commands.CreateUser;
using Application.Shared.Abstractions;
using Domain.Features.Orders.Entities;
using Domain.Features.Orders.Repository;
using Domain.Features.Users.Repository;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, OrderResponse>
{
    private readonly IOrderRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CreateOrderCommandHandler> _logger;
    private readonly IValidator<CreateOrderCommand> _validator;

    public CreateOrderCommandHandler(
        ILogger<CreateOrderCommandHandler> logger,
        IOrderRepository repository,
        IValidator<CreateOrderCommand> validator,
        IUserRepository userRepository)
    {
        _repository = repository;
        _logger = logger;
        _validator = validator;
        _userRepository = userRepository;
    }

    public async Task<Result<OrderResponse>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Creating an Order {request.UserId}");

        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

        var userValidate = await CreateUserCommandValidator.ValidateUser(_userRepository, request.UserId);
        if (userValidate.IsFailed) return Result.Fail(userValidate.Errors);

        var openedOrder = await _repository.GetOpenedOrdersByUser(request.UserId);
        if (openedOrder.Any()) return Result.Fail(new List<string> { "Usuário já possui uma ordem aberta" });

        // -------------------------------------------------------------
        // TODO: Bater no sistema de pagamento primeiro
        // Só continuar se houver dinheiro na conta do usuário
        // -------------------------------------------------------------

        var user = userValidate.Value;
        var order = Order.Create(null, user, request.requestedTime, request.address, DateTime.Now);

        order.SetAmount(request.amount);
        order.SetDistanceInKM(request.totalDiscance);
        order.SetDurationInSeconds(request.totalDuration);

        // Save user
        var savedOrder = await _repository.SaveAsync(order);

        _logger.LogInformation($"Order Created {savedOrder.Id}");
        return Result.Ok((OrderResponse)savedOrder);
    }
}
