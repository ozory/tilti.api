using Application.Features.Users.Contracts;
using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Users.Commands.RefreshToken;

public record RefreshTokenCommand(string Token, string RefreshToken) : ICommand<Result<AuthenticationResponse>>;


