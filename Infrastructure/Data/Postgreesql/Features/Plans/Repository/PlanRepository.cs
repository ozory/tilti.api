using System.Collections.Immutable;
using Domain.Features.Plans.Entities;
using Domain.Features.Plans.Repository;
using Domain.Features.Users.Repository;
using Infrastructure.Data.Postgreesql.Features.Users.Maps;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Postgreesql.Features.Users.Repository;

public class PlanRepository : IPlanRepository
{
    private readonly TILTContext _context;

    public PlanRepository(TILTContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Plan>> GetAllAsync()
    {
        var plans = await _context.Plans.AsNoTracking().ToListAsync();
        var userList = plans.Select(u => u.ToDomainPlan()).ToImmutableList();

        return userList!;
    }

    public async Task<Plan?> GetByIdAsync(long id)
    {
        var plan = await _context.Plans.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        return plan?.ToDomainPlan() ?? null;
    }

    public async Task<Plan?> GetPlanByNameOrAmount(string name, decimal amount)
    {
        var plan = await _context.Plans.AsNoTracking()
            .FirstOrDefaultAsync(u =>
            u.Name.ToLower() == name.ToLower() ||
            u.Amount == amount);
        return plan?.ToDomainPlan() ?? null;
    }

    public async Task<Plan> SaveAsync(Plan entity)
    {
        var plan = entity.ToPersistencePlan();
        _context.Add(plan);
        await _context.SaveChangesAsync();
        return plan?.ToDomainPlan()!;
    }

    public async Task<Plan> UpdateAsync(Plan entity)
    {
        var plan = entity.ToPersistencePlan();
        _context.Update(plan);
        await _context.SaveChangesAsync();
        return plan?.ToDomainPlan()!;
    }
}
