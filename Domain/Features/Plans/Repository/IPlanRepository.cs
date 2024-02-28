using Domain.Abstractions;
using Domain.Features.Plans.Entities;

namespace Domain.Features.Plans.Repository;

public interface IPlanRepository : IGenericRepository<Plan>
{
    Task<Plan?> GetPlanByNameOrAmount(string Name, decimal amount);
}
