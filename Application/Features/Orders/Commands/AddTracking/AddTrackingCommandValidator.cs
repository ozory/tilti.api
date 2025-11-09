using FluentValidation;

namespace Application.Features.Orders.Commands.AddTracking;

public class AddTrackingCommandValidator : AbstractValidator<AddTrackingCommand>
{
    public AddTrackingCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0)
            .WithMessage("OrderId must be greater than 0");

        RuleFor(x => x.DriverId)
            .GreaterThan(0)
            .WithMessage("DriverId must be greater than 0");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90)
            .WithMessage("Latitude must be between -90 and 90");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180)
            .WithMessage("Longitude must be between -180 and 180");
    }
}