using Domain.Features.Users.Entities;
using Domain.Features.Users.Repository;
using FluentResults;
using FluentValidation;

namespace Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator(IUserRepository userRepository)
    {
        RuleFor(s => s.Name).NotNull().NotEmpty().WithMessage("Name is required");
        RuleFor(s => s.Email).NotNull().ChildRules(s => s.RuleFor(x => x).EmailAddress()).WithMessage("Email is required");
        RuleFor(s => s.Document).NotNull().NotEmpty().WithMessage("Document is required");

        RuleFor(s => s.Email)
            .MustAsync(async (email, _) =>
            {
                var user = await userRepository.FirstOrDefault(u => (string)u.Email! == email);
                return user == null;
            })
        .WithMessage("Email allready in use");

        RuleFor(s => s.Document)
            .MustAsync(async (document, _) =>
            {
                var user = await userRepository.FirstOrDefault(u => (string)u.Document! == document);
                return user == null;
            })
        .WithMessage("Document allready in use");

        RuleFor(s => s.Password).NotNull().ChildRules(s => s.RuleFor(x => x).NotNull().NotEmpty()).WithMessage("Password is required");
    }

    public static async Task<Result<User>> ValidateUser(IUserRepository userRepository, long userId)
    {

        var user = await userRepository.GetByIdAsync(userId);

        if (user == null) return Result.Fail("User not Found");
        if (user.Status != Domain.Enums.UserStatus.Active) return Result.Fail("User not active");

        return user;
    }
}
