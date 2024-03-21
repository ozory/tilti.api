using System.Collections.Immutable;
using Application.Features.Orders.Contracts;
using Application.Features.Users.Commands.CreateUser;
using Application.Shared.Abstractions;
using Domain.Enums;
using Domain.Features.Orders.Entities;
using Domain.Features.Orders.Events;
using Domain.Features.Orders.Repository;
using Domain.Features.Users.Repository;
using Domain.Shared.Abstractions;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Commands.CloseOrder;

public class CloseExpiredOrdersCommandHandler : ICommandHandler<CloseExpireOrdersCommand, Result<ImmutableList<OrderResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CloseExpiredOrdersCommandHandler> _logger;
    private readonly IValidator<CloseExpireOrdersCommand> _validator;
    private readonly ICacheRepository _cacheRepository;

    public CloseExpiredOrdersCommandHandler(
        ILogger<CloseExpiredOrdersCommandHandler> logger,
        IValidator<CloseExpireOrdersCommand> validator,
        IUnitOfWork unitOfWork,
        ICacheRepository cacheRepository)
    {
        _logger = logger;
        _validator = validator;
        _unitOfWork = unitOfWork;
        _cacheRepository = cacheRepository;
    }

    public async Task<Result<Result<ImmutableList<OrderResponse>>>> Handle(
        CloseExpireOrdersCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Invalidating old olders {requestedTime}", request.requestedTime);

        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

        var openedOrder = await _unitOfWork.OrderRepository.GetOpenedOrdersThatExpired(request.requestedTime);

        var currentTime = DateTime.Now;
        var tasks = openedOrder.ToList().Select(async order =>
        {
            order!.SetStatus(OrderStatus.Expired);
            order.SetUpdated(currentTime);

            // Remove from all cacheds
            await _cacheRepository.RemoveAsync(order.Id.ToString());
            await _cacheRepository.RemoveAsync($"order-user-{order.UserId}");

            return (OrderResponse)order;
        });

        var result = await Task.WhenAll(tasks);
        return Result.Ok(result.ToImmutableList());
    }
}
