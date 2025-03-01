using Application.Features.Users.Contracts;
using Application.Shared.Abstractions;
using Domain.Enums;
using Domain.Shared.Abstractions;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Commands.RegisterValidation;

public class RegisterValidationCommandHandler
    : ICommandHandler<RegisterValidationCommand, UserResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RegisterValidationCommandHandler> _logger;
    private readonly IValidator<RegisterValidationCommand> _validator;
    private readonly ISecurityExtensions _securityExtensions;

    public RegisterValidationCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<RegisterValidationCommandHandler> logger,
        IValidator<RegisterValidationCommand> validator,
        ISecurityExtensions securityExtensions)
    {
        this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._validator = validator ?? throw new ArgumentNullException(nameof(validator));
        this._securityExtensions = securityExtensions ?? throw new ArgumentNullException(nameof(securityExtensions));
    }

    private readonly string className = nameof(RegisterValidationCommandHandler);

    public async Task<Result<UserResponse>> Handle(RegisterValidationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{className}] Validating user registration code for UserId {UserId}", className, request.UserId);

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

        var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
        if (user == null) return Result.Fail("User not found");

        if (user.VerificationCode != request.ConfirmationCode)
        {
            _logger.LogWarning("[{className}] Invalid confirmation code for UserId {UserId}", className, request.UserId);
            return Result.Fail("Invalid confirmation code");
        }

        user.SetStatus(UserStatus.Active);
        user.SetVerificationCode(null);

        var savedUser = await _unitOfWork.UserRepository.UpdateAsync(user);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("[{className}] User {UserId} registration validated successfully", className, request.UserId);
        return Result.Ok((UserResponse)savedUser);
    }
}