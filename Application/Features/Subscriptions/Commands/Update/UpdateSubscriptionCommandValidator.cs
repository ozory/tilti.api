using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Plans.Enums;
using FluentValidation;

namespace Application.Features.Subscriptions.Commands.Update;

public class UpdateSubscriptionCommandValidator : AbstractValidator<UpdateSubscriptionCommand>
{
    public UpdateSubscriptionCommandValidator()
    {
        this.ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(s => s.SubscriptionId)
           .LessThanOrEqualTo(0)
           .WithMessage("PlanId is required");

        RuleFor(s => s.PlanId)
           .LessThanOrEqualTo(0)
           .WithMessage("PlanId is required");

        RuleFor(s => (int)s.Status)
            .InclusiveBetween(1, 3)
           .WithMessage("PlanId is required");

        RuleFor(s => s.DueDate)
            .LessThan(DateTime.Now)
            .Must(BeAValidDate)
           .WithMessage("Invalid DueDate");
    }

    private bool BeAValidDate(DateTime date)
    {
        return !date.Equals(default(DateTime));
    }

}

