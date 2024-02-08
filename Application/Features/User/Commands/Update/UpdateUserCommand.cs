using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.User.Contracts;
using Application.Shared.Abstractions;
using DomainUser = Domain.Features.Users.Entities.User;
namespace Application.Features.User.Commands.UpdateUser;

public sealed record UpdateUserCommand
(
    long id,
    string Name,
    string Email,
    string Document,
    string? Password,
    ushort? Status

) : ICommand<UserResponse>;


