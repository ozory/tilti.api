using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.User.Repository;
using FluentValidation;
using DomainUser = Domain.Features.User.Entities.User;

namespace Application.Features.User.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator(IUserRepository userRepository)
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