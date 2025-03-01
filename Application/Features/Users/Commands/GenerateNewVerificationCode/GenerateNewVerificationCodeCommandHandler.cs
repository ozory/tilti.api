using Application.Shared.Abstractions;
using Domain.Enums;
using Domain.Features.Users.Events;
using Domain.Shared.Abstractions;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Commands.GenerateNewVerificationCode;

public class GenerateNewVerificationCodeCommandHandler : ICommandHandler<GenerateNewVerificationCodeCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GenerateNewVerificationCodeCommandHandler> _logger;
    private readonly IValidator<GenerateNewVerificationCodeCommand> _validator;
    private readonly ISecurityExtensions _securityExtensions;

    public GenerateNewVerificationCodeCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<GenerateNewVerificationCodeCommandHandler> logger,
        IValidator<GenerateNewVerificationCodeCommand> validator,
        ISecurityExtensions securityExtensions)
    {
        this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._validator = validator ?? throw new ArgumentNullException(nameof(validator));
        this._securityExtensions = securityExtensions ?? throw new ArgumentNullException(nameof(securityExtensions));
    }

    private readonly string className = nameof(GenerateNewVerificationCodeCommandHandler);

    public async Task<Result> Handle(GenerateNewVerificationCodeCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{className}] Handling forget password for email {Email}", className, request.Email);

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

        var user = await _unitOfWork.UserRepository.FirstOrDefault(x => x.Email.Value == request.Email);
        if (user == null) return Result.Fail("User not found");

        user.SetVerificationCode(_securityExtensions.ComputeValidationCode());
        user.SetStatus((UserStatus)request.Status);

        var savedUser = await _unitOfWork.UserRepository.UpdateAsync(user);
        savedUser.AddDomainEvent(UserGenerateNewVerificationCodeEvent.Create(user));
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("[{className}] Password reset successfully for user {UserId}", className, user.Id);
        return Result.Ok();
    }
}
