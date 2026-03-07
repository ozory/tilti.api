using Application.Features.Users.Contracts;
using Application.Shared.Abstractions;
using FluentResults;
namespace Application.Features.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand
(
    long id,
    string Name,
    string Email,
    string Document,
    string? Password,
    ushort? Status

) : ICommand<Result<UserResponse>>;


