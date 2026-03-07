using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Users.Commands.GenerateNewVerificationCode;

public sealed record GenerateNewVerificationCodeCommand(
    string Email,
    ushort Status
) : ICommand<Result>;
