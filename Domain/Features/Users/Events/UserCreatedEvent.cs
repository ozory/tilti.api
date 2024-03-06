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

    public UserCreatedDomainEvent(User user)
    {
        this.Id = user.Id;
        this.Name = user.Name.Value!;
        this.Email = user.Email.Value!;
        this.Document = user.Document.Value!;
        this.DriveEnable = user.DriveEnable;
        this.CreatedAt = user.CreatedAt;
    }
}