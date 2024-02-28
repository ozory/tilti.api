using FluentValidation;

namespace Application.Features.Users.Commands.RefreshToken;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        this.ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(s => s.Token)
        .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .NotNull()
        .WithMessage("Token is required");

        RuleFor(s => s.RefreshToken)
        .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .NotNull()
        .WithMessage("RefreshToken is required");
    }
}
