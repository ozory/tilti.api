using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstractions;
using Domain.Plans.Entities;

namespace Domain.Features.Plans.Repository;

public interface IPlanRepository : IGenericRepository<Plan>
{

}
