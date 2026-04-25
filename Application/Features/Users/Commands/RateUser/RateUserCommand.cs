using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Users.Commands.RateUser;

public sealed record RateUserCommand(
    long OrderId,
    long SourceUserId,
    long TargetUserId,
    float Value,
    string? Description = null,
    string? Tags = null
) : ICommand<Result<bool>>;
