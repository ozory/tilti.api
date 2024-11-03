using Domain.Features.Users.Repository;
using FluentValidation;

namespace Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    /// <summary>
    /// Validator for the CreateOrderCommand.
    /// </summary>
    /// <param name="userRepository">The user repository to validate user-related rules.</param>
    /// <remarks>
    /// This validator ensures that:
    /// <list type="bullet">
    /// <item><description>UserId is greater than or equal to 0.</description></item>
    /// <item><description>RequestedTime is greater than today's date.</description></item>
    /// <item><description>Addresses are not null and contain more than one address.</description></item>
    /// </list>
    /// </remarks>
    public CreateOrderCommandValidator(IUserRepository userRepository)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(s => s.UserId).GreaterThanOrEqualTo(0).WithMessage("UserId is required");
        RuleFor(s => s.RequestedTime).GreaterThan(d => DateTime.Today).WithMessage("It's not possible to request in specific time");
        RuleFor(s => s.Addresses).NotNull().Must(x => x.Count > 1).WithMessage("The order must have two address, from A point to B point");
    }
}
