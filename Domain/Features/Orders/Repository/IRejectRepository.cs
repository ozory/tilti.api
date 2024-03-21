using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstractions;
using Domain.Features.Orders.Entities;

namespace Domain.Features.Orders.Repository;

public interface IRejectRepository : IGenericRepository<Rejection>
{
    Task<IReadOnlyList<Rejection?>> GetRejectionsByUser(long driverId);
}
