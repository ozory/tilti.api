using Domain.Features.Users.Repository;
using FluentValidation;
namespace Application.Features.Orders.Commands.AddMessage;

public class AddMessageCommandValidator : AbstractValidator<AddMessageCommand>
{
    public AddMessageCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(s => s.SourceUserId).GreaterThanOrEqualTo(0).WithMessage("UserId is required");
        RuleFor(s => s.TargetUserId).GreaterThanOrEqualTo(0).WithMessage("UserId is required");
        RuleFor(s => s.Message).NotEmpty().WithMessage("Message is required");
    }
}
