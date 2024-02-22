using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainUser = Domain.Features.Users.Entities.User;

namespace Application.Features.Users.Contracts;

public sealed record AuthenticationResponse(
    long? Id,
    string? Name,
    string? Email,
    string? Token,
    string? RefreshToken)
{ }
