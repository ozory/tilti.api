using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Shared.Abstractions;
using Domain.Enums;
using Domain.Features.Orders.Entities;
using Domain.Features.Orders.Repository;
using Domain.Features.Users.Entities;
using Infrastructure.Data.Postgreesql.Shared;

namespace Infrastructure.Data.Postgreesql.Features.Orders.Repository;

public class RejectionRepository :
    GenericRepository<Rejection>,
    IRejectRepository
{
    private readonly ICacheRepository _cacheRepository;

    public RejectionRepository(
        TILTContext context,
        ICacheRepository cacheRepository) : base(context)
    {
        _cacheRepository = cacheRepository;
    }

    private readonly string IncludeProperties = $"{nameof(User)},{nameof(Order)}";

    public async Task<IReadOnlyList<Rejection?>> GetRejectionsByUser(long driverId)
    {
        var rejections = await Filter(
            filter: s => s.DriverId == driverId
            && s.CreatedAt >= DateTime.Now.AddMinutes(-7)
            && s.Order.Status == OrderStatus.ReadyToAccept
            , includeProperties: IncludeProperties);

        return rejections;
    }

}
