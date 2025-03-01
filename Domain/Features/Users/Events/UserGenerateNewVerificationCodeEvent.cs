using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Users.Entities;
using Domain.Shared.Abstractions;

namespace Domain.Features.Users.Events;

public class UserGenerateNewVerificationCodeEvent : IDomainEvent
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string VerificationCode { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public static UserGenerateNewVerificationCodeEvent Create(User user)
    {
        return new UserGenerateNewVerificationCodeEvent
        {
            Id = user.Id,
            Name = user.Name.Value!,
            VerificationCode = user.VerificationCode!,
            Email = user.Email.Value!,
            CreatedAt = user.CreatedAt
        };
    }
}