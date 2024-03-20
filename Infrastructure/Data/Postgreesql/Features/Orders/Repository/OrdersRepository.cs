using Application.Shared.Abstractions;
using Domain.Enums;
using Domain.Features.Orders.Entities;
using Domain.Features.Orders.Events;
using Domain.Features.Orders.Repository;
using Domain.Features.Users.Entities;
using Infrastructure.Data.Postgreesql.Shared;
using NetTopologySuite.Geometries;

namespace Infrastructure.Data.Postgreesql.Features.Orders.Repository;

public class OrdersRepository :
    GenericRepository<Order>,
    IOrderRepository
{
    private readonly ICacheRepository _cacheRepository;
    public OrdersRepository(
        TILTContext context,
        ICacheRepository cacheRepository) : base(context)
    {
        _cacheRepository = cacheRepository;
    }

    private readonly string IncludeProperties = $"{nameof(User)}";

    public async Task<IReadOnlyList<Order?>> GetOpenedOrdersByUser(long idUser)
    {
        var status = Enum.GetValues(typeof(OrderStatus)).Cast<ushort>().ToList();

        var orders = await Filter(
            u => u.UserId == idUser && status.Contains((ushort)u.Status),
            includeProperties: nameof(User));

        return orders!;
    }

    public async Task<IReadOnlyList<Order?>> GetOrdersByPoint(Point point)
    {
        // Returning directily from database
        // var orders = await Filter(u => u.Point!.Distance(point) < 1000, includeProperties: nameof(User));
        // return orders!;

        // Returning directily from cache
        var orders = await _cacheRepository.GetNearObjects<OrderCreatedDomainEvent>(point.X, point.Y);
        return orders.Select(x => (Order)x!).ToList();
    }
}