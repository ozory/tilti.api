using Domain.Features.Users.Repository;
using FluentValidation;

namespace Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator(IUserRepository userRepository)
    {
        RuleFor(s => s.Name)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Name is required");

        RuleFor(s => s.Email)
            .NotNull()
            .ChildRules(s => s.RuleFor(x => x).EmailAddress())
            .WithMessage("Email is required");

        RuleFor(s => s.Email)
        .MustAsync(async (email, _) =>
            {
                var user = await userRepository.GetByEmail(email);
                return user == null;
            })
        .WithMessage("Email allready in use");

        RuleFor(s => s.Document)
            .NotNull()
            .NotEmpty()
            .WithMessage("Document is required");

        RuleFor(s => s.Document)
        .MustAsync(async (document, _) =>
            {
                var user = await userRepository.GetByDocument(document);
                return user == null;
            })
        .WithMessage("Document allready in use");

        RuleFor(s => s.Password)
            .NotNull()
            .ChildRules(s => s.RuleFor(x => x)
                .NotNull()
                .NotEmpty())
            .WithMessage("Password is required");
    }
}
