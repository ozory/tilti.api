using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstractions;
using Domain.Features.Orders.Entities;

namespace Domain.Features.Orders.Repository;

public interface ITrackingRepository : IGenericRepository<Tracking>
{
    Task<IReadOnlyList<Tracking>> GetTrackingByOrder(long idOrder, long trackingId, CancellationToken cancellationToken);
}
