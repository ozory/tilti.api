using Application.Features.Users.Contracts;
using Application.Shared.Abstractions;

namespace Application.Features.Users.Commands.RegisterValidation;

public sealed record RegisterValidationCommand(
    long UserId,
    string ConfirmationCode
) : ICommand<UserResponse>;