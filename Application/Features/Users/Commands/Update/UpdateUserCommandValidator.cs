using Domain.Features.Users.Repository;
using FluentValidation;

namespace Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator(IUserRepository userRepository)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(s => s.Name)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Name is required");

        RuleFor(s => s.Email)
            .NotNull()
            .ChildRules(s => s.RuleFor(x => x).EmailAddress())
            .WithMessage("Email is required");

        RuleFor(s => s.id)
        .MustAsync(async (source, _) =>
            {
                var user = await userRepository.GetByIdAsync(source);
                return user != null;
            })
        .WithMessage("User not found");

        RuleFor(s => new { s.id, s.Email })
        .MustAsync(async (source, _) =>
            {
                var user = await userRepository.FirstOrDefault(u => u.Email.Value == source.Email);
                if (user != null)
                {
                    return user.Id == source.id;
                };
                return true;
            })
        .WithMessage("Email allready in use");

        RuleFor(s => s.Document)
            .NotNull()
            .NotEmpty()
            .WithMessage("Document is required");

        RuleFor(s => new { s.id, s.Document })
        .MustAsync(async (source, _) =>
            {
                var user = await userRepository.FirstOrDefault(u => u.Document.Value == source.Document);
                if (user != null)
                {
                    return user.Id == source.id;
                };
                return true;
            })
        .WithMessage("Document allready in use");

        When(s => !string.IsNullOrEmpty(s.Password), () =>
        {
            RuleFor(s => s.Password)
                        .NotNull()
                        .ChildRules(s => s.RuleFor(x => x)
                            .NotNull()
                            .NotEmpty())
                        .WithMessage("Password is required");
        });

    }
}
