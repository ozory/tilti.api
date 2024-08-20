using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstractions;
using Domain.Features.Users.Entities;

namespace Domain.Features.Users.Repository;

public interface IRateRepository : IGenericRepository<Rate>
{

}
