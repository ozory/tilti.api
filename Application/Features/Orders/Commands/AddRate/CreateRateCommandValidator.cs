using Domain.Features.Users.Entities;
using Domain.Features.Users.Repository;
using FluentResults;
using FluentValidation;

namespace Application.Features.Orders.Commands.AddRate;

public class CreateRateCommandValidator : AbstractValidator<CreateRateCommand>
{
    public CreateRateCommandValidator()
    {
        RuleFor(s => s.SourceUserId).GreaterThan(0).WithMessage("SourceUserId is required");
        RuleFor(s => s.TargetUserId).GreaterThan(0).WithMessage("TargetUserId is required");
        RuleFor(s => s.Value).GreaterThan(0).NotEmpty().WithMessage("Value is required");
    }
}
