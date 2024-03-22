using Domain.Features.Users.Repository;
using FluentValidation;

namespace Application.Features.Orders.Commands.UpdateOrder;

public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator(IUserRepository userRepository)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

    }
}
