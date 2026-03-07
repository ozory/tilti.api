using Application.Features.Users.Contracts;
using Application.Shared.Abstractions;
using FluentResults;
namespace Application.Features.Security.Commands.Authenticate;

public sealed record AuthenticateUserCommand
(
    string Email,
    string Password

) : ICommand<Result<AuthenticationResponse>>;


