using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstractions;
using Domain.Features.Payments.Enums;
using Domain.Features.Users.Entities;
using Domain.ValueObjects;

namespace Domain.Features.Payments.Entities;

public class Payment : Entity<Payment>
{
    public string? Identifier { get; protected set; } = null!;
    public User User { get; protected set; } = null!;
    public Amount Amount { get; protected set; } = null!;
    public PaymentStatus Status { get; protected set; } = PaymentStatus.Pending;
}
