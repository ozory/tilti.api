using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Plans.Repository;
using Domain.Plans.Entities;
using Infrastructure.Data.Postgreesql.Shared;

namespace Infrastructure.Data.Postgreesql.Features.Plans.Repository;

public class PlanRepository : GenericRepository<Plan>, IPlanRepository
{
    public PlanRepository(TILTContext context) : base(context)
    {
    }
}
