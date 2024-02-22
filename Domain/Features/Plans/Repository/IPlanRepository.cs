using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstractions;
using Domain.Features.Plans.Entities;

namespace Domain.Features.Plans.Repository;

public interface IPlanRepository : IGenericRepository<Plan>
{
    Task<Plan?> GetPlanByNameOrAmount(string Name, decimal amount);
}
