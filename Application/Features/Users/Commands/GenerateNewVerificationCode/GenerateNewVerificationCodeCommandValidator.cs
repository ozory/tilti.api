using FluentValidation;

namespace Application.Features.Users.Commands.GenerateNewVerificationCode;

public class GenerateNewVerificationCodeCommandValidator : AbstractValidator<GenerateNewVerificationCodeCommand>
{
    public GenerateNewVerificationCodeCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Valid email is required");
    }
}