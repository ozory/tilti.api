using Domain.Features.Users.Entities;
using Domain.Features.Users.Repository;
using FluentResults;
using FluentValidation;

namespace Application.Features.Users.Commands.Rate;

public class CreateRateCommandValidator : AbstractValidator<CreateRateCommand>
{
    public CreateRateCommandValidator()
    {
        RuleFor(s => s.sourceUserId).GreaterThan(0).WithMessage("SourceUserId is required");
        RuleFor(s => s.targetUserId).GreaterThan(0).WithMessage("TargetUserId is required");
        RuleFor(s => s.value).GreaterThan(0).NotEmpty().WithMessage("Value is required");
    }
}
