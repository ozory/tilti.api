using Domain.Features.Plans.Entities;
using Domain.Features.Plans.Repository;
using Infrastructure.Data.Postgreesql.Shared;

namespace Infrastructure.Data.Postgreesql.Features.Plans.Repository;

public class PlanRepository :
    GenericRepository<Plan>,
    IPlanRepository
{
    public PlanRepository(TILTContext context) : base(context) { }

    public async Task<Plan?> GetPlanByNameOrAmount(string name, decimal amount)
        => await FirstOrDefault(u =>
            u.Name.Value!.ToLower() == name.ToLower() ||
            u.Amount.Value == amount);
}
