using Application.Features.Orders.Contracts;
using Application.Shared.Abstractions;
using Domain.Enums;
using Domain.Features.Orders.Entities;
using Domain.Features.Orders.Events;
using Domain.Features.Orders.Repository;
using Domain.Features.Users.Entities;
using Domain.Shared.ValueObjects;
using Infrastructure.Data.Postgreesql.Shared;
using Microsoft.Extensions.Configuration;
using NetTopologySuite.Geometries;

namespace Infrastructure.Data.Postgreesql.Features.Orders.Repository;

public class OrdersRepository :
    GenericRepository<Order>,
    IOrderRepository
{
    private readonly ICacheRepository _cacheRepository;
    private readonly IConfiguration _configuration;

    public OrdersRepository(
        TILTContext context,
        ICacheRepository cacheRepository,
        IConfiguration configuration) : base(context)
    {
        _cacheRepository = cacheRepository;
        _configuration = configuration;
    }

    private readonly string IncludeProperties = $"{nameof(User)}";

    public async Task<IReadOnlyList<Order?>> GetOpenedOrdersByUser(long idUser)
    {
        var status = Enum.GetValues(typeof(OrderStatus)).Cast<ushort>().ToList();

        var orders = await Filter(
            u => u.UserId == idUser && status.Contains((ushort)u.Status),
            includeProperties: IncludeProperties);

        return orders!;
    }

    public async Task<IReadOnlyList<Order?>> GetOrdersByPoint(Point point)
    {
        var orders = await _cacheRepository.GetNearObjects<OrderResponse>(point.X, point.Y);
        if (orders == null || orders.Count == 0)
        {
            var rangeInKM = int.Parse(_configuration.GetSection("Configurations:RangeInKM").Value!);

            var ordersFromBase = await Filter(u => u.Point!.Distance(point) < rangeInKM, includeProperties: nameof(User));

            if (ordersFromBase != null && ordersFromBase.Count > 0)
                await _cacheRepository.GeoAdd(ordersFromBase.Select(x => (OrderResponse)x).ToList());

            return ordersFromBase!;
        }
        return orders.Select(x => (Order)x!).ToList();
    }

    public async Task<IReadOnlyList<Order?>> GetOpenedOrdersThatExpired(DateTime expireTime)
    {
        var orders = await Filter(
            u => u.CreatedAt < expireTime && u.Status == OrderStatus.ReadyToAccept,
            includeProperties: IncludeProperties);
        return orders!;
    }
}