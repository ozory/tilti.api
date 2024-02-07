using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Plans.Entities;
using Domain.Plans.Enums;
using FluentValidation;

namespace Domain.Plans.Validators;

public class PlanValidator : AbstractValidator<Plan>
{
    public PlanValidator()
    {
        RuleFor(x => x)
            .NotNull()
            .WithMessage("Plan is null");

        RuleFor(x => x.Status)
            .NotEqual(PlanStatus.Active)
            .WithMessage("Plan must be active");

        RuleFor(x => x.Amount.Value)
            .LessThanOrEqualTo(0)
            .WithMessage("Plan value must positive");
    }
}
