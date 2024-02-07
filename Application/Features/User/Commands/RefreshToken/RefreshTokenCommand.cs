using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.User.Contracts;
using Application.Shared.Abstractions;

namespace Application.Features.User.Commands.RefreshToken;

public record RefreshTokenCommand(string Token, string RefreshToken) : ICommand<AuthenticationResponse>;


