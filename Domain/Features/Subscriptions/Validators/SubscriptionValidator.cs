using Domain.Entities;
using DomainSubscription = Domain.Features.Subscription.Entities.Subscription;
using Domain.Plans.Enums;
using Domain.Plans.Validators;
using Domain.Subscriptions.Enums;
using FluentValidation;

namespace Domain.Validators;

public class SubscriptionValidator : AbstractValidator<DomainSubscription>
{
    public SubscriptionValidator()
    {
        RuleFor(s => s.Plan)
            .SetValidator(new PlanValidator());

        RuleFor(s => s.User)
            .NotNull()
            .WithMessage("Customer is required");

        RuleFor(s => s.User.Id)
            .LessThanOrEqualTo(0)
            .WithMessage("Customer Id must be greater than zero");

        When(s => s.Status == SubscriptionStatus.Active, () =>
        {
            RuleFor(s => s.User.Status)
            .NotEqual(Enums.UserStatus.Active)
            .WithMessage("Customer must be Active");

            RuleFor(s => s.Plan.Status)
            .NotEqual(PlanStatus.Active)
            .WithMessage("Plan must be Active");
        });
    }
}
