using Application.Features.Users.Contracts;
using Application.Shared.Abstractions;
using Domain.Enums;
using Domain.Features.Users.Entities;
using Domain.Features.Users.Repository;
using Domain.Shared.Abstractions;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<UpdateUserCommandHandler> logger,
    IValidator<UpdateUserCommand> validator,
    ISecurityExtensions securityExtensions) : ICommandHandler<UpdateUserCommand, UserResponse>
{

    public async Task<Result<UserResponse>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Validando usuário {request.Email}");

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

        var userRepository = unitOfWork.GetUserRepository();
        var user = await userRepository.GetByIdAsync(request.id);
        if (user == null) return Result.Fail("User not found");

        user.SetName(request.Name);
        user.SetEmail(request.Email);
        user.SetDocument(request.Document);

        if (request.Status is not null && (ushort)user.Status != request.Status)
            user.SetStatus((UserStatus)request.Status);

        // Generate Hashed Password
        if (!string.IsNullOrEmpty(request.Password))
        {
            user.SetVerificationSalt(securityExtensions.GenerateSalt());
            user.SetPassword(securityExtensions.ComputeHash(user.VerificationSalt!, request.Password));
        }

        logger.LogInformation($"Atualizando usuário {request.Email}");
        var savedUser = await userRepository.UpdateAsync(user);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.LogInformation($"Usuário {request.Email} atualizado com sucesso");
        return Result.Ok((UserResponse)savedUser);
    }
}
