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

    private readonly string className = nameof(CreateUserCommandHandler);

    public async Task<Result<UserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("[{className}] Validando usuário {email}", className, request.Email);

            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

            logger.LogInformation("[{className}]  Criando usuário {email}", className, request.Email);

            var user = User.Create(null, request.Name, request.Email, request.Document, request.Password, DateTime.Now);

            // Security Info
            var securitySalt = securityExtensions.GenerateSalt();
            user.SetVerificationSalt(securitySalt);
            user.SetPassword(securityExtensions.ComputeHash(securitySalt, request.Password));
            user.SetVerificationCode(securityExtensions.ComputeValidationCode());

            var savedUser = await unitOfWork.UserRepository.SaveAsync(user);
            savedUser.AddDomainEvent(UserCreatedDomainEvent.Create(user));

            await unitOfWork.CommitAsync(cancellationToken);

            logger.LogInformation("[{className}] Usuário {email} criado com sucesso", className, request.Email);
            return Result.Ok((UserResponse)savedUser);
        }
        catch (Exception ex)
        {
            logger.LogError("[{className}] Error on creating User : {request} Error: {ex}", className, request, ex);
            throw;
        }

    }
}
