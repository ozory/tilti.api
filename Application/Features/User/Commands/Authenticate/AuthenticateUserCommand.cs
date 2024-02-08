using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.User.Contracts;
using Application.Shared.Abstractions;
using DomainUser = Domain.Features.Users.Entities.User;
namespace Application.Features.User.Commands.CreateUser;

public sealed record AuthenticateUserCommand
(
    string Email,
    string Password

) : ICommand<AuthenticationResponse>;


