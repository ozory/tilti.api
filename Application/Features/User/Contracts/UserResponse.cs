using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainUser = Domain.Features.User.Entities.User;

namespace Application.Features.User.Contracts;

public record UserResponse
(
    long? Id,
    string? Name,
    string? Email,
    string? Document,
    DateTime? CreatedAt,
    DateTime? UpdatedAt,
    string Status
)
{
    public static implicit operator UserResponse(DomainUser user)
        => new UserResponse(
                user.Id,
                user.Name.Value,
                user.Email.Value,
                user.Document.Value,
                user.CreatedAt,
                user.UpdatedAt,
                user.Status.ToString());
}