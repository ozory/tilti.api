using Application.Features.Orders.Contracts;
using Application.Features.Users.Commands.CreateUser;
using Application.Shared.Abstractions;
using Domain.Features.Orders.Entities;
using Domain.Features.Orders.Repository;
using Domain.Features.Users.Repository;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Commands.PrecifyOrder;

public class PrecifyOrderCommandHandler : ICommandHandler<PrecifyOrderCommand, OrderResponse>
{
    private readonly IMapServices _mapServices;
    private readonly IOrderRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<PrecifyOrderCommandHandler> _logger;
    private readonly IValidator<PrecifyOrderCommand> _validator;
    private readonly ISecurityExtensions _securityExtensions;

    public PrecifyOrderCommandHandler(
        ILogger<PrecifyOrderCommandHandler> logger,
        IOrderRepository repository,
        IValidator<PrecifyOrderCommand> validator,
        ISecurityExtensions securityExtensions,
        IUserRepository userRepository,
        IMapServices mapServices)
    {
        _repository = repository;
        _logger = logger;
        _validator = validator;
        _securityExtensions = securityExtensions;
        _userRepository = userRepository;
        _mapServices = mapServices;
    }

    public async Task<Result<OrderResponse>> Handle(
        PrecifyOrderCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Precifying an Order {request.UserId}");

        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

        var userValidate = await CreateUserCommandValidator.ValidateUser(_userRepository, request.UserId);
        if (userValidate.IsFailed) return Result.Fail(userValidate.Errors);

        var user = userValidate.Value;
        var currentDate = DateTime.Now;
        var order = Order.Create(null, user, currentDate, request.address, currentDate);

        order = await _mapServices.CalculateOrderAsync(order);

        return Result.Ok((OrderResponse)order);
    }
}
