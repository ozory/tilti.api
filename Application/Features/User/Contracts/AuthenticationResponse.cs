using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainUser = Domain.Features.User.Entities.User;

namespace Application.Features.User.Contracts;

public sealed record AuthenticationResponse(
    long? Id,
    string? Name,
    string? Email,
    string? Token)
{
    public static implicit operator AuthenticationResponse(DomainUser user)
   => new AuthenticationResponse(
           user.Id,
           user.Name.Value,
           user.Email.Value,
           null
           );
}
