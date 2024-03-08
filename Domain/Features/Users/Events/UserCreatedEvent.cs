using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Users.Entities;
using Domain.Shared.Abstractions;

namespace Domain.Features.Users.Events;

public record UserCreatedDomainEvent : IDomainEvent
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Document { get; set; } = null!;
    public Boolean DriveEnable { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public static UserCreatedDomainEvent Create(User user)
    {
        return new UserCreatedDomainEvent
        {
            Id = user.Id,
            Name = user.Name.Value!,
            Email = user.Email.Value!,
            Document = user.Document.Value!,
            DriveEnable = user.DriveEnable,
            CreatedAt = user.CreatedAt
        };
    }
}