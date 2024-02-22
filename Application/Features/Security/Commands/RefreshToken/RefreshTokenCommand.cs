using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Users.Contracts;
using Application.Shared.Abstractions;

namespace Application.Features.Users.Commands.RefreshToken;

public record RefreshTokenCommand(string Token, string RefreshToken) : ICommand<AuthenticationResponse>;


