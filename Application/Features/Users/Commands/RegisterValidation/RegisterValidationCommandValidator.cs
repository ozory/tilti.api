using FluentValidation;

namespace Application.Features.Users.Commands.RegisterValidation;

public class RegisterValidationCommandValidator : AbstractValidator<RegisterValidationCommand>
{
    public RegisterValidationCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("UserId is required");

        RuleFor(x => x.ConfirmationCode)
            .NotEmpty()
            .WithMessage("Confirmation code is required");
    }
}