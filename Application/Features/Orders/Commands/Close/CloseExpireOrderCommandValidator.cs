using Domain.Features.Users.Repository;
using FluentValidation;

namespace Application.Features.Orders.Commands.CloseOrder;

public class CloseExpireOrderCommandValidator : AbstractValidator<CloseExpireOrdersCommand>
{
    public CloseExpireOrderCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(s => s.requestedTime).GreaterThan(d => DateTime.Today).WithMessage("It's not possible to request in specific time");
    }
}
