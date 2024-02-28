using FluentValidation;

namespace Application.Features.Plans.Commands.Create;

public class CreatePlanCommandValidator : AbstractValidator<CreatePlanCommand>
{
    public CreatePlanCommandValidator()
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
