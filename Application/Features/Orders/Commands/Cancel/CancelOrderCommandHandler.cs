using Application.Features.Orders.Contracts;
using Application.Features.Users.Commands.CreateUser;
using Application.Shared.Abstractions;
using Domain.Enums;
using Domain.Features.Orders.Entities;
using Domain.Features.Orders.Repository;
using Domain.Features.Users.Repository;
using Domain.Orders.Enums;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandler : ICommandHandler<CancelOrderCommand, OrderResponse>
{
    private readonly IOrderRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CancelOrderCommandHandler> _logger;
    private readonly IValidator<CancelOrderCommand> _validator;
    private readonly string className = nameof(CancelOrderCommandHandler);


    public CancelOrderCommandHandler(
        ILogger<CancelOrderCommandHandler> logger,
        IOrderRepository repository,
        IValidator<CancelOrderCommand> validator,
        IUserRepository userRepository)
    {
        _repository = repository;
        _logger = logger;
        _validator = validator;
        _userRepository = userRepository;
    }

    public async Task<Result<OrderResponse>> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{className}] Cancelling an Order {Id}", className, request.OrderId);

        try
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

            var userValidate = await CreateUserCommandValidator.ValidateUser(_userRepository, request.UserId);
            if (userValidate.IsFailed) return Result.Fail(userValidate.Errors);

            var openedOrder = await _repository.GetByIdAsync(request.OrderId);
            if (openedOrder is null) return Result.Fail(new List<string> { "Nenhum pedido encontrado" });

            if (openedOrder.Status == OrderStatus.Canceled) return Result.Fail("Pedido já foi cancelado");
            if (openedOrder.Status == OrderStatus.Finished) return Result.Fail("Pedido já foi Finalizado");

            openedOrder.SetStatus(OrderStatus.Canceled);
            openedOrder.SetCancelDescription(request.description);
            openedOrder.SetCancelRasons(string.Join(",", request.reason));

            // Save user
            var canceledOrder = await _repository.UpdateAsync(openedOrder);

            _logger.LogInformation("[{className}] Order Canceled {Id}", className, request.OrderId);
            return Result.Ok((OrderResponse)canceledOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error Canceling Order : {request} Error: {ex}", className, request, ex);
            throw;
        }

    }
}
