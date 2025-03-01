using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Shared.Abstractions;
using Domain.Features.Orders.Entities;
using Domain.Features.Orders.Repository;
using Infrastructure.Data.Postgreesql.Shared;

namespace Infrastructure.Data.Postgreesql.Features.Orders.Repository;

public class TrackingRepository : GenericRepository<Tracking>,
    ITrackingRepository
{
    private readonly ICacheRepository _cacheRepository;

    public TrackingRepository(
        TILTContext context,
        ICacheRepository cacheRepository) : base(context)
    {
        _cacheRepository = cacheRepository;
    }

    public async Task<IReadOnlyList<Tracking>> GetTrackingByOrder(long idOrder, long trackingId, CancellationToken cancellationToken)
    {
        return await Filter(
           filter: s => s.OrderId == idOrder
           && s.Id > trackingId
           , includeProperties: "");
    }
}
