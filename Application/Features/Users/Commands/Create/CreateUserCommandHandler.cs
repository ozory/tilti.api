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

public class CreateUserCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<CreateUserCommandHandler> logger,
    IValidator<CreateUserCommand> validator,
    ISecurityExtensions securityExtensions) : ICommandHandler<CreateUserCommand, UserResponse>
{

    public async Task<Result<UserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Validando usuário {email}", request.Email);

        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

        logger.LogInformation("Criando usuário {email}", request.Email);

        var user = User.Create(null, request.Name, request.Email, request.Document, request.Password, DateTime.Now);

        // Security Info
        var securitySalt = securityExtensions.GenerateSalt();
        user.SetVerificationSalt(securitySalt);
        user.SetPassword(securityExtensions.ComputeHash(securitySalt, request.Password));
        user.SetVerificationCode(securityExtensions.ComputeValidationCode());

        var savedUser = await unitOfWork.UserRepository.SaveAsync(user);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.LogInformation("Usuário {email} criado com sucesso", request.Email);
        return Result.Ok((UserResponse)savedUser);
    }
}
