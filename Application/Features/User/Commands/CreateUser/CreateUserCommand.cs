using Application.Features.User.Contracts;
using Application.Shared.Abstractions;
namespace Application.Features.User.Commands.CreateUser;

public sealed record CreateUserCommand
(
    string Name,
    string Email,
    string Document,
    string Password

) : ICommand<UserResponse>;


