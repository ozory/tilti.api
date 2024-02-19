using System;
using Domain.Features.Users.Repository;
using FluentValidation;

namespace Application.Features.Orders.Commands.PrecifyOrder;

public class PrecifyOrderCommandValidator : AbstractValidator<PrecifyOrderCommand>
{
    public PrecifyOrderCommandValidator(IUserRepository userRepository)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(s => s.UserId).GreaterThanOrEqualTo(0).WithMessage("UserId is required");
        RuleFor(s => s.address).NotNull().Must(x => x.Count > 1).WithMessage("The order must have two address, from A point to B point");
    }
}
