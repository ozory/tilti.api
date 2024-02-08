using Domain.Abstractions;
using Domain.Enums;
using Domain.Plans.Enums;
using Domain.ValueObjects;
using FluentResults;

namespace Domain.Features.Plans.Entities;

/// <summary>
/// Representa um plano
/// </summary>
public class Plan : Entity<Plan>
{
    public Name Name { get; private set; } = null!;
    public Description Description { get; private set; } = null!;
    public Amount Amount { get; private set; } = null!;
    public PlanStatus Status { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="amount"></param>
    /// <param name="status"></param>
    private Plan(long? id, Name name, Description description, Amount amount, PlanStatus status)
    {
        Id = id ?? 0;
        Name = name;
        Description = description;
        Amount = amount;
        Status = status;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="amount"></param>
    /// <param name="status"></param>
    /// <param name="createdDate"></param>
    /// <returns></returns>
    public static Plan CreatePlan(
        long? id,
        string name,
        string description,
        decimal amount,
        ushort status,
        DateTime? createdDate)
    {
        var plan = new Plan(
            id,
            new Name(name),
            new Description(description),
            new Amount(amount),
            (PlanStatus)status);

        plan.CreatedAt = createdDate ?? DateTime.Now;
        return plan;
    }
}
