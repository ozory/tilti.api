using System.Collections.Immutable;
using Domain.Enums;
using Domain.Features.Orders.Entities;
using Domain.Features.Orders.Repository;
using Domain.Features.Users.Entities;
using Domain.Shared.ValueObjects;
using Infrastructure.Data.Postgreesql.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NetTopologySuite.Geometries;
using DomainOrder = Domain.Features.Orders.Entities.Order;

namespace Infrastructure.Data.Postgreesql.Features.Orders.Repository;

public class OrdersRepository :
    GenericRepository<Order>,
    IOrderRepository
{
    private readonly IDistributedCache _distributedCache;
    public OrdersRepository(
        TILTContext context,
        IDistributedCache distributedCache) : base(context)
    {
        _distributedCache = distributedCache;
    }

    private readonly string IncludeProperties = $"{nameof(User)}";

    public override async Task<DomainOrder> SaveAsync(DomainOrder entity)
    {
        entity = await base.SaveAsync(entity);

       // _distributedCache.Set
        return entity;
    }

    public async Task<IReadOnlyList<Order?>> GetOpenedOrdersByUser(long idUser)
    {
        var status = Enum.GetValues(typeof(OrderStatus)).Cast<ushort>().ToList();

        var orders = await Filter(
            u => u.UserId == idUser && status.Contains((ushort)u.Status),
            includeProperties: nameof(User));

        return orders!;
    }

    public async Task<IReadOnlyList<DomainOrder?>> GetOrdersByPoint(Point point)
    {
        var orders = await Filter(
            u => u.Point!.Distance(point) < 1000,
            includeProperties: nameof(User));

        return orders!;
    }
}