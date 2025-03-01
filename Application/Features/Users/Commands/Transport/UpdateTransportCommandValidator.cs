using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Users.Repository;
using FluentResults;
using FluentValidation;

namespace Application.Features.Users.Commands.Transport;

public class UpdateTransportCommandValidator : AbstractValidator<UpdateTransportCommand>
{
    public UpdateTransportCommandValidator(IUserRepository userRepository)
    {
        RuleFor(s => s.Name).NotNull().NotEmpty().WithMessage("Name is required");
        RuleFor(s => s.Model).NotNull().NotEmpty().WithMessage("Model is required");
        RuleFor(s => s.Plate).NotNull().NotEmpty().WithMessage("Model is required");
        RuleFor(s => (int)s.Year).LessThanOrEqualTo(0).WithMessage("Year is required");

        RuleFor(s => s.Plate)
            .MustAsync(async (plate, _) =>
            {
                var user = await userRepository.FirstOrDefault(u => (string)u.Transport!.Plate! == plate);
                return user == null;
            })
        .WithMessage("Plate allready in use");
    }
}
