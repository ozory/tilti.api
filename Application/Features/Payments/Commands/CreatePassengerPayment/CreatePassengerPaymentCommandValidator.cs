using FluentValidation;

namespace Application.Features.Payments.Commands.CreatePassengerPayment;

/// <summary>
/// Validator for CreatePassengerPaymentCommand
/// </summary>
public class CreatePassengerPaymentCommandValidator : AbstractValidator<CreatePassengerPaymentCommand>
{
    /// <summary>
    /// Initializes a new instance of the validator
    /// </summary>
    public CreatePassengerPaymentCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("User ID must be greater than 0");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0");

        RuleFor(x => x.ExternalReference)
            .NotEmpty().WithMessage("External reference is required");

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.Now).WithMessage("Due date must be in the future");
    }
}
