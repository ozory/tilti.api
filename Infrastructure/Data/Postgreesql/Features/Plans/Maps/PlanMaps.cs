using Domain.Enums;
using Infrastructure.Data.Postgreesql.Features.Plans.Entities;
using Infrastructure.Data.Postgreesql.Features.Users.Entities;
using DomainPlan = Domain.Features.Plans.Entities.Plan;

namespace Infrastructure.Data.Postgreesql.Features.Users.Maps;

public static class PlanMaps
{
    internal static Plan ToPersistencePlan(this DomainPlan plan)
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

    internal static DomainPlan ToDomainPlan(this Plan plan)
    {
        var domainUser = DomainPlan.CreatePlan(
            plan.Id,
            plan.Name,
            plan.Description,
            plan.Amount,
            plan.Status,
            plan.Created);

        domainUser.SetUpdatedAt(plan.Updated);

        return domainUser;
    }
}
