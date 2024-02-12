using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Shared.Abstractions;
using Domain.Features.Users.Repository;
using FluentValidation;

namespace Application.Features.Users.Commands.RefreshToken;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        this.ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(s => s.Token)
        .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .NotNull()
        .WithMessage("Token is required");

        RuleFor(s => s.RefreshToken)
        .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .NotNull()
        .WithMessage("RefreshToken is required");
    }
}
