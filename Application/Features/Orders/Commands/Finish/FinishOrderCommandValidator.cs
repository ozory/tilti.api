using FluentValidation;

namespace Application.Features.Orders.Commands.FinishOrder;

/// <summary>
/// Validator for the FinishOrderCommand.
/// </summary>
public class FinishOrderCommandValidator : AbstractValidator<FinishOrderCommand>
{
    /// <summary>
    /// Validator ensures that:
    /// <list type="bullet">
    /// <item><description>OrderId is greater than 0.</description></item>
    /// <item><description>UserId is greater than 0.</description></item>
    /// </list>
    /// </summary>
    public FinishOrderCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(s => s.OrderId).GreaterThan(0).WithMessage("OrderId is required");
        RuleFor(s => s.UserId).GreaterThan(0).WithMessage("UserId is required");
    }
}