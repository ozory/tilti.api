using Domain.Features.Users.Repository;
using FluentValidation;

namespace Application.Features.Security.Commands.Authenticate;

public class AuthenticateUserCommandValidator : AbstractValidator<AuthenticateUserCommand>
{
    public AuthenticateUserCommandValidator(IUserRepository userRepository)
    {
        RuleFor(s => s.Email)
            .NotNull()
            .ChildRules(s => s.RuleFor(x => x).EmailAddress())
            .WithMessage("Email is required");

        RuleFor(s => s.Email)
        .MustAsync(async (email, _) =>
            {
                var user = await userRepository.GetByEmail(email);
                if (user == null || user.Status != Domain.Enums.UserStatus.Active) return false;
                return true;
            })
        .WithMessage("Email not found");

        RuleFor(s => s.Password)
            .NotNull()
            .ChildRules(s => s.RuleFor(x => x)
                .NotNull()
                .NotEmpty())
            .WithMessage("Password is required");
    }
}
