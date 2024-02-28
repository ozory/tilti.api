using FluentValidation;

namespace Application.Features.Plans.Commands.Update;

public class UpdatePlanCommandValidator : AbstractValidator<UpdatePlanCommand>
{
    public UpdatePlanCommandValidator()
    {
        this.ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(s => s.Name)
               .Cascade(CascadeMode.Stop)
                   .NotEmpty()
                   .NotNull()
               .WithMessage("Name is required");

        RuleFor(s => s.Description)
        .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .NotNull()
        .WithMessage("RefreshToken is required");

        RuleFor(s => s.Amount)
        .Cascade(CascadeMode.Stop)
            .GreaterThan(0)
        .WithMessage("Amount must be greater than ZERO");
    }
}
