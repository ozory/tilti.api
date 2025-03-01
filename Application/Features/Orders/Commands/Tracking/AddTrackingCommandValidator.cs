using Application.Features.Orders.Commands.OrderTracking;
using Domain.Features.Users.Repository;
using FluentValidation;
namespace Application.Features.Orders.Commands.OrderTracking;

public class AddTrackingCommandValidator : AbstractValidator<AddTrackingCommand>
{
    public AddTrackingCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(s => s.OrderId).GreaterThanOrEqualTo(0).WithMessage("OrderId is required");
    }
}
