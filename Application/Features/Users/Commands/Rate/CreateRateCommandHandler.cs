using Application.Features.Users.Contracts;
using Application.Shared.Abstractions;
using Domain.Features.Users.Entities;
using Domain.Features.Users.Events;
using Domain.Features.Users.Repository;
using Domain.Shared.Abstractions;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Commands.CreateUser;

public class CreateRateCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<CreateRateCommandHandler> logger,
    IValidator<CreateRateCommand> validator
    ) : ICommandHandler<CreateRateCommand, bool>
{

    private readonly string className = nameof(CreateUserCommandHandler);

    public async Task<Result<bool>> Handle(CreateRateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("[{className}] Validando usuário ", className);

            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

            var sourceUser = await CreateUserCommandValidator.ValidateUser(unitOfWork.UserRepository, request.sourceUserId);
            if (sourceUser.IsFailed) return Result.Fail(sourceUser.Errors);

            var targetUser = await CreateUserCommandValidator.ValidateUser(unitOfWork.UserRepository, request.targetUserId);
            if (targetUser.IsFailed) return Result.Fail(targetUser.Errors);

            var exists = await unitOfWork.RateRepository.Filter(x => x.SourceUserId == request.sourceUserId && x.TargetUserId == request.targetUserId);
            if (exists.Any()) return Result.Fail("Rate already exists");

            // Security Info
            var rate = Rate.Create(sourceUser.Value, targetUser.Value, request.value);

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
