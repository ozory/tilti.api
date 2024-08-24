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
