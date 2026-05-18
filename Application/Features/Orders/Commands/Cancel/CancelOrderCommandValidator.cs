using Application.Features.Orders.Commands.CancelOrder;
using Domain.Features.Users.Repository;
using FluentValidation;

namespace Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderCommandValidator(IUserRepository userRepository)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(s => s.UserId)
            .GreaterThanOrEqualTo(0)
            .WithMessage("UserId is required");

        RuleFor(s => s.OrderId)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Order is required");

        RuleForEach(s => s.reason)
            .NotEmpty().WithMessage("Reason items must not be empty")
            .MaximumLength(100).WithMessage("Reason items must be at most 100 characters");

        RuleFor(s => s.description)
            .MaximumLength(500)
            .WithMessage("Description must be at most 500 characters");

        RuleFor(s => s.cancelledBy)
            .Must(x => x == null || x.Equals("User", StringComparison.OrdinalIgnoreCase) || x.Equals("Driver", StringComparison.OrdinalIgnoreCase))
            .WithMessage("CancelledBy must be null, 'User', or 'Driver'");
    }
}
