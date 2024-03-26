using Application.Features.Orders.Contracts;
using Application.Features.Users.Commands.CreateUser;
using Application.Shared.Abstractions;
using Domain.Enums;
using Domain.Features.Orders.Entities;
using Domain.Features.Orders.Repository;
using Domain.Features.Users.Repository;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Commands.UpdateOrder;

public class UpdateOrderCommandHandler : ICommandHandler<UpdateOrderCommand, OrderResponse>
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<UpdateOrderCommandHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IValidator<UpdateOrderCommand> _validator;
    private readonly ISecurityExtensions _securityExtensions;
    private readonly string className = nameof(UpdateOrderCommandHandler);


    public UpdateOrderCommandHandler(
        ILogger<UpdateOrderCommandHandler> logger,
        IOrderRepository repository,
        IValidator<UpdateOrderCommand> validator,
        ISecurityExtensions securityExtensions,
        IUserRepository userRepository)
    {
        _repository = repository;
        _logger = logger;
        _validator = validator;
        _securityExtensions = securityExtensions;
        _userRepository = userRepository;
    }

    public async Task<Result<OrderResponse>> Handle(
        UpdateOrderCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{className}] Updating an Order {id}", className, request.OrderId);

        try
        {

            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

            var userValidate = await CreateUserCommandValidator.ValidateUser(_userRepository, request.UserId);
            if (userValidate.IsFailed) return Result.Fail(userValidate.Errors);

            var openedOrder = await _repository.GetByIdAsync(request.OrderId);
            if (openedOrder is null) return Result.Fail(new List<string> { "Nenhum pedido encontrado" });

            var user = userValidate.Value;
            var order = Order.Create(
                request.OrderId,
                user,
                request.requestedTime,
                request.address,
                openedOrder.CreatedAt);

            order.SetAmount(request.amount);
            order.SetDistanceInKM(request.totalDiscance);
            order.SetDurationInSeconds(request.totalDuration);


            // Save user
            var updatedOrder = await _repository.UpdateAsync(order);

            _logger.LogInformation("[{className}] Order Updated {Id}", className, updatedOrder.Id);
            return Result.Ok((OrderResponse)updatedOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error Updating Order : {request} Error: {ex}", className, request, ex);
            throw;
        }

    }
}
