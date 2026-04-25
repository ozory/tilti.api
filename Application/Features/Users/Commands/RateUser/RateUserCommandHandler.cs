using Application.Shared.Abstractions;
using Domain.Enums;
using Domain.Features.Orders.Entities;
using Domain.Shared.Abstractions;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Commands.RateUser;

public class RateUserCommandHandler(
    IUnitOfWork unitOfWork,
    IValidator<RateUserCommand> validator,
    ILogger<RateUserCommandHandler> logger) : ICommandHandler<RateUserCommand, bool>
{
    private readonly string className = nameof(RateUserCommandHandler);

    public async Task<Result<bool>> Handle(RateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("[{className}] Validando avaliação do pedido {OrderId}", className, request.OrderId);

            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            var order = await unitOfWork.OrderRepository.GetByIdAsync(request.OrderId);
            if (order is null) return Result.Fail("Order not found");

            var sourceUser = await unitOfWork.UserRepository.GetByIdAsync(request.SourceUserId);
            if (sourceUser is null) return Result.Fail("Source user not found");
            if (sourceUser.Status != UserStatus.Active) return Result.Fail("Source user not active");

            var targetUser = await unitOfWork.UserRepository.GetByIdAsync(request.TargetUserId);
            if (targetUser is null) return Result.Fail("Target user not found");
            if (targetUser.Status != UserStatus.Active) return Result.Fail("Target user not active");

            if (order.DriverId is null) return Result.Fail("Order has no driver");

            var isUserRatingDriver =
                request.SourceUserId == order.UserId
                && request.TargetUserId == order.DriverId.Value;

            var isDriverRatingUser =
                request.SourceUserId == order.DriverId.Value
                && request.TargetUserId == order.UserId;

            if (!isUserRatingDriver && !isDriverRatingUser)
            {
                return Result.Fail("Rate must be between order user and order driver");
            }

            if (isUserRatingDriver && !targetUser.DriveEnable)
            {
                return Result.Fail("Target user is not a driver");
            }

            var existingRates = await unitOfWork.RateRepository.Filter(x =>
                x.OrderId == request.OrderId
                && x.SourceUserId == request.SourceUserId
                && x.TargetUserId == request.TargetUserId);

            if (existingRates.Any()) return Result.Fail("Rate already exists");

            var rate = Rate.Create(
                order,
                sourceUser,
                targetUser,
                request.Value,
                request.Description,
                request.Tags);

            await unitOfWork.RateRepository.SaveAsync(rate);
            await unitOfWork.CommitAsync(cancellationToken);

            logger.LogInformation(
                "[{className}] Avaliação cadastrada com sucesso para pedido {OrderId} ({SourceUserId} -> {TargetUserId})",
                className,
                request.OrderId,
                request.SourceUserId,
                request.TargetUserId);
            return Result.Ok(true);
        }
        catch (Exception ex)
        {
            logger.LogError("[{className}] Error while rating user: {request} Error: {ex}", className, request, ex);
            throw;
        }
    }
}
