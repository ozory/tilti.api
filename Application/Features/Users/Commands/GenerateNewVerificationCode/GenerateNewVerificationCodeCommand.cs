using Application.Shared.Abstractions;

namespace Application.Features.Users.Commands.GenerateNewVerificationCode;

public sealed record GenerateNewVerificationCodeCommand(
    string Email,
    ushort Status
) : ICommand;
