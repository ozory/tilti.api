using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Application.Features.Subscriptions.Commands.Create
{
    public class CreateSubscriptionCommandValidator : AbstractValidator<CreateSubscriptionCommand>
    {
        public CreateSubscriptionCommandValidator()
        {
            this.ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(s => s.userId)
               .GreaterThanOrEqualTo(0)
               .WithMessage("UserId is required");

            RuleFor(s => s.planId)
               .GreaterThanOrEqualTo(0)
               .WithMessage("PlanId is required");
        }

    }
}