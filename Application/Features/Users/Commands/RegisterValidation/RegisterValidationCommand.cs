using Application.Features.Users.Contracts;
using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Users.Commands.RegisterValidation;

public sealed record RegisterValidationCommand(
    long UserId,
    string ConfirmationCode
) : ICommand<Result<UserResponse>>;