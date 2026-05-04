using FluentValidation;
using Domain.Subscriptions.Enums;

namespace Application.Features.Subscriptions.Commands.CreateSubscription;

/// <summary>
/// Validator for CreateSubscriptionCommand
/// </summary>
public class CreateSubscriptionCommandValidator : AbstractValidator<CreateSubscriptionCommand>
{
    public CreateSubscriptionCommandValidator()
    {
        RuleFor(x => x.userId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");

        RuleFor(x => x.planId)
            .GreaterThan(0)
            .WithMessage("Plan ID must be greater than 0");

        RuleFor(x => x.subscriptionType)
            .Must(type => type == SubscriptionType.Driver || type == SubscriptionType.Passenger)
            .WithMessage("SubscriptionType must be 'Driver' or 'Passenger'");
    }
}