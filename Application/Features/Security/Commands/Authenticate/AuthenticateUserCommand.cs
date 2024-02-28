using Application.Features.Users.Contracts;
using Application.Shared.Abstractions;
namespace Application.Features.Security.Commands.Authenticate;

public sealed record AuthenticateUserCommand
(
    string Email,
    string Password

) : ICommand<AuthenticationResponse>;


