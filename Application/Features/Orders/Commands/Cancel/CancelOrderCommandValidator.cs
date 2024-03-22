using Application.Features.Orders.Commands.CancelOrder;
using Domain.Features.Users.Repository;
using FluentValidation;

namespace Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderCommandValidator(IUserRepository userRepository)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(s => s.UserId).GreaterThanOrEqualTo(0).WithMessage("UserId is required");
        RuleFor(s => s.OrderId).GreaterThanOrEqualTo(0).WithMessage("Order is required");
        RuleFor(s => s.reason).Must(x => x == null || !x.Any());

    }
}
