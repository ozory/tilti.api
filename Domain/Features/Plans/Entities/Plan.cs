using Domain.Abstractions;
using Domain.Enums;
using Domain.Plans.Enums;
using Domain.ValueObjects;
using FluentResults;

namespace Domain.Plans.Entities;

/// <summary>
/// Representa um plano
/// </summary>
public class Plan : Entity<Plan>
{
    public Name Name { get; private set; } = null!;
    public Description Description { get; private set; } = null!;
    public Amount Amount { get; private set; } = null!;
    public PlanStatus Status { get; private set; }
}
