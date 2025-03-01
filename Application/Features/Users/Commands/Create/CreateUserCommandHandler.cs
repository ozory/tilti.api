using Application.Features.Users.Contracts;
using Application.Shared.Abstractions;
using Domain.Enums;
using Domain.Features.Users.Entities;
using Domain.Features.Users.Events;
using Domain.Features.Users.Repository;
using Domain.Shared.Abstractions;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, UserResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateUserCommandHandler> _logger;
    private readonly IValidator<CreateUserCommand> _validator;
    private readonly ISecurityExtensions _securityExtensions;

    public CreateUserCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<CreateUserCommandHandler> logger,
    IValidator<CreateUserCommand> validator,
    ISecurityExtensions securityExtensions)
    {
        this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._validator = validator ?? throw new ArgumentNullException(nameof(validator));
        this._securityExtensions = securityExtensions ?? throw new ArgumentNullException(nameof(securityExtensions));
    }

    private readonly string className = nameof(CreateUserCommandHandler);

    public async Task<Result<UserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("[{className}] Validando usuário {email}", className, request.Email);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (validationResult?.IsValid == false) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

            _logger.LogInformation("[{className}]  Criando usuário {email}", className, request.Email);

            var user = User.Create(null, request.Name, request.Email, request.Document, request.Password, DateTime.Now);

            // Security Info
            var securitySalt = _securityExtensions.GenerateSalt();
            user.SetVerificationSalt(securitySalt);
            user.SetPassword(_securityExtensions.ComputeHash(securitySalt, request.Password));
            user.SetVerificationCode(_securityExtensions.ComputeValidationCode());
            user.SetDriveEnable(request.DriveEnable);
            user.SetStatus(UserStatus.PendingRegisterConfirmation);

            var savedUser = await _unitOfWork.UserRepository.SaveAsync(user);
            savedUser.AddDomainEvent(UserCreatedDomainEvent.Create(user));

            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("[{className}] Usuário {email} criado com sucesso", className, request.Email);
            return Result.Ok((UserResponse)savedUser);
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error on creating User : {request} Error: {ex}", className, request, ex);
            throw;
        }

    }
}
