using Infrastructure.Data.Postgreesql.Features.Subscriptions.Entities;
using Infrastructure.Data.Postgreesql.Shared.Abstractions;
using DomainPlan = Domain.Features.Plans.Entities.Plan;

namespace Infrastructure.Data.Postgreesql.Features.Plans.Entities;

public class Plan : InfrastructureEntity
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;

    public Decimal Amount { get; set; }
    public ushort Status { get; set; }

    public DateTime Created { get; set; }
    public DateTime Updated { get; set; } = DateTime.Now;

    public ICollection<Subscription>? Subscriptions { get; } = [];

    public static explicit operator Plan(DomainPlan plan)
    {
        Plan persistencePlan = new Plan
        {
            Id = plan.Id,
            Name = plan.Name.Value!,
            Description = plan.Description.Value!,
            Status = (ushort)plan.Status,
            Amount = plan.Amount.Value,
            Created = plan.CreatedAt,
        };

        return persistencePlan;
    }

    public static explicit operator DomainPlan(Plan? plan)
    {
        if (plan is null) return null!;
        var domainPlan = DomainPlan.Create(
            plan.Id,
            plan.Name,
            plan.Description,
            plan.Amount,
            plan.Status,
            plan.Created);

        domainPlan.SetUpdatedAt(plan.Updated);

        return domainPlan;
    }

}
