using FluentValidation;

namespace Application.Features.Subscriptions.Commands.CreateDriverSubscription;

/// <summary>
/// Validator for CreateDriverSubscriptionCommand
/// </summary>
public class CreateDriverSubscriptionCommandValidator : AbstractValidator<CreateDriverSubscriptionCommand>
{
    public CreateDriverSubscriptionCommandValidator()
    {
        RuleFor(x => x.userId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");

        RuleFor(x => x.planId)
            .GreaterThan(0)
            .WithMessage("Plan ID must be greater than 0");
    }
}