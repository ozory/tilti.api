using Application.Features.Users.Commands.CreateUser;
using Application.Shared.Abstractions;
using Domain.Features.Orders.Entities;
using Domain.Shared.Abstractions;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;
namespace Application.Features.Orders.Commands.AddRate;

public class CreateRateCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<CreateRateCommandHandler> logger,
    IValidator<CreateRateCommand> validator
    ) : ICommandHandler<CreateRateCommand, bool>
{

    private readonly string className = nameof(CreateRateCommandHandler);

    /// <summary>
    /// Handles the creation of a new rate for an order between two users.
    /// </summary>
    /// <param name="request">The command containing rate creation details including order ID, source user ID, target user ID, rate value, description and tags.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
    /// <returns>A Result containing a boolean indicating success (true) or failure with error messages.</returns>
    /// <remarks>
    /// The method performs the following steps:
    /// 1. Validates the request using the validator
    /// 2. Checks if the order exists
    /// 3. Validates both source and target users
    /// 4. Verifies if a rate already exists for the given combination
    /// 5. Creates and saves the new rate
    /// </remarks>
    /// <exception cref="Exception">Thrown when an unexpected error occurs during rate creation.</exception>
    public async Task<Result<bool>> Handle(CreateRateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("[{className}] Validando usuário ", className);

            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

            var order = await unitOfWork.OrderRepository.GetByIdAsync(request.OrderId);
            if (order is null) return Result.Fail("Order not found");

            var sourceUser = await CreateUserCommandValidator.ValidateUser(unitOfWork.UserRepository, request.SourceUserId);
            if (sourceUser.IsFailed) return Result.Fail(sourceUser.Errors);

            var targetUser = await CreateUserCommandValidator.ValidateUser(unitOfWork.UserRepository, request.TargetUserId);
            if (targetUser.IsFailed) return Result.Fail(targetUser.Errors);

            var exists = await unitOfWork.RateRepository.Filter(
                x => x.SourceUserId == request.SourceUserId
                && x.TargetUserId == request.TargetUserId
                && x.OrderId == request.OrderId);

            if (exists.Any()) return Result.Fail("Rate already exists");

            // Security Info
            var rate = Rate.Create(
                order,
                sourceUser.Value,
                targetUser.Value,
                request.Value,
                request.Description,
                request.Tags);

            // Save user
            var savedRate = await unitOfWork.RateRepository.SaveAsync(rate);
            await unitOfWork.CommitAsync(cancellationToken);

            logger.LogInformation("[{className}] Avaliação cadastrada com sucesso", className);
            return Result.Ok(true);
        }
        catch (Exception ex)
        {
            logger.LogError("[{className}] Error on creating User : {request} Error: {ex}", className, request, ex);
            throw;
        }

    }
}
