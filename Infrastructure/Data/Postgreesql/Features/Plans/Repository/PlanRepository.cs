using System.Collections.Immutable;
using Domain.Features.Plans.Entities;
using Domain.Features.Plans.Repository;
using Infrastructure.Data.Postgreesql.Shared;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;

using InfrastructurePlan = Infrastructure.Data.Postgreesql.Features.Plans.Entities.Plan;

namespace Infrastructure.Data.Postgreesql.Features.Plans.Repository;

public class PlanRepository :
    GenericRepository<InfrastructurePlan>,
    IPlanRepository
{
    public PlanRepository(TILTContext context) : base(context) { }

    public new async Task<Plan?> GetByIdAsync(long id)
        => (Plan)await base.GetByIdAsync(id);

    public new async Task<IReadOnlyList<Plan>> GetAllAsync()
    => (await base.GetAllAsync()).Select(p => (Plan)p).ToImmutableList();

    public async Task<Plan?> GetPlanByNameOrAmount(string name, decimal amount)
    {
        var plan = await _context.Plans.AsNoTracking()
            .FirstOrDefaultAsync(u =>
            u.Name.ToLower() == name.ToLower() ||
            u.Amount == amount);
        return (Plan?)plan;
    }

    public async Task<Plan> SaveAsync(Plan entity)
        => (Plan)await base.SaveAsync((InfrastructurePlan)entity);

    public async Task<Plan> UpdateAsync(Plan entity)
        => (Plan)await base.UpdateAsync((InfrastructurePlan)entity);
}
