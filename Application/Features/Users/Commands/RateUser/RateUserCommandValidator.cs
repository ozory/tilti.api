using FluentValidation;

namespace Application.Features.Users.Commands.RateUser;

public class RateUserCommandValidator : AbstractValidator<RateUserCommand>
{
    public RateUserCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0)
            .WithMessage("OrderId is required");

        RuleFor(x => x.SourceUserId)
            .GreaterThan(0)
            .WithMessage("SourceUserId is required");

        RuleFor(x => x.TargetUserId)
            .GreaterThan(0)
            .WithMessage("TargetUserId is required");

        RuleFor(x => x.Value)
            .GreaterThan(0)
            .WithMessage("Value is required");

        RuleFor(x => x)
            .Must(x => x.SourceUserId != x.TargetUserId)
            .WithMessage("Source and target users must be different");
    }
}
