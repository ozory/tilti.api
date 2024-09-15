using Application.Features.Users.Contracts;
using Application.Shared.Abstractions;
namespace Application.Features.Users.Commands.CreateUser;

public sealed record CreateUserCommand
(
    string Name,
    string Email,
    string Document,
    string Password,
    Boolean DriveEnable = false

) : ICommand<UserResponse>;


