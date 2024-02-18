using System;
using Domain.Features.Users.Repository;
using FluentValidation;

namespace Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator(IUserRepository userRepository)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(s => s.UserId).GreaterThanOrEqualTo(0).WithMessage("UserId is required");
        RuleFor(s => s.requestedTime).GreaterThan(d => DateTime.Today).WithMessage("It's not possible to request in specific time");
        RuleFor(s => s.address).NotNull().Must(x => x.Count > 1).WithMessage("The order must have two address, from A point to B point");
    }
}
