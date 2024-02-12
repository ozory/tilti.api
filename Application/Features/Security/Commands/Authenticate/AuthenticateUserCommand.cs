using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Users.Contracts;
using Application.Shared.Abstractions;
using DomainUser = Domain.Features.Users.Entities.User;
namespace Application.Features.Security.Commands.CreateUser;

public sealed record AuthenticateUserCommand
(
    string Email,
    string Password

) : ICommand<AuthenticationResponse>;


