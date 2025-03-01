using Application.Features.Orders.Contracts;
using Application.Shared.Abstractions;
<<<<<<< HEAD
using Domain.Enums;
=======
>>>>>>> 7b148c2b8ae7f452586616b92a970a41fd43347c
using Domain.Features.Orders.Entities;
using Domain.Features.Orders.Events;
using Domain.Features.Orders.Repository;
using Domain.Features.Users.Entities;
<<<<<<< HEAD
=======
using Domain.Orders.Enums;
using Domain.Shared.Enums;
>>>>>>> 7b148c2b8ae7f452586616b92a970a41fd43347c
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

<<<<<<< HEAD
=======
    public int RangeInKM
    {
        get
        { return int.Parse(_configuration.GetSection("Configurations:RangeInKM").Value!); }
    }

>>>>>>> 7b148c2b8ae7f452586616b92a970a41fd43347c
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

<<<<<<< HEAD
    public async Task<IReadOnlyList<Order?>> GetOrdersByPoint(Point point)
    {
        var orders = await _cacheRepository.GetNearObjects<OrderResponse>(point.X, point.Y);
        if (orders == null || orders.Count == 0)
        {
            var rangeInKM = int.Parse(_configuration.GetSection("Configurations:RangeInKM").Value!);

            var ordersFromBase = await Filter(u => u.Point!.Distance(point) < rangeInKM, includeProperties: nameof(User));
=======
    public async Task<IReadOnlyList<Order?>> GetOrdersByPoint(Point point, Point? destinationPoint, OrderType orderType)
    {
        var orders = await _cacheRepository.GetNearObjects<OrderResponse>(point.X, point.Y);
        var filtredByTypeOrders = orders.Where(x => x?.OrderType == orderType
        && (destinationPoint == null ||
            CalculateDistance(x!.Location.Latitude, x.Location.Longitude, destinationPoint.X, destinationPoint.Y) < RangeInKM)).ToList();

        if (filtredByTypeOrders == null || filtredByTypeOrders.Count == 0)
        {
            var ordersFromBase = await Filter(
                u => u.Point!.Distance(point) < RangeInKM
                && u.Type == orderType
                && (destinationPoint == null ||
                    CalculateDistance(u.Location.Latitude, u.Location.Longitude, destinationPoint.X, destinationPoint.Y) < RangeInKM),
                includeProperties: IncludeProperties);
>>>>>>> 7b148c2b8ae7f452586616b92a970a41fd43347c

            if (ordersFromBase != null && ordersFromBase.Count > 0)
                await _cacheRepository.GeoAdd(ordersFromBase.Select(x => (OrderResponse)x).ToList());

            return ordersFromBase!;
        }
<<<<<<< HEAD
        return orders.Select(x => (Order)x!).ToList();
=======
        return filtredByTypeOrders.Select(x => (Order)x!).ToList();
>>>>>>> 7b148c2b8ae7f452586616b92a970a41fd43347c
    }

    public async Task<IReadOnlyList<Order?>> GetOpenedOrdersThatExpired(DateTime expireTime)
    {
        var orders = await Filter(
            u => u.CreatedAt < expireTime && u.Status == OrderStatus.ReadyToAccept,
            includeProperties: IncludeProperties);
        return orders!;
    }
<<<<<<< HEAD
=======

    private double CalculateDistance(double lat1, double lng1, double lat2, double lng2)
    {
        const double R = 6371; // Raio da Terra em quilômetros

        // Converter graus para radianos
        double lat1Rad = DegreesToRadians(lat1);
        double lng1Rad = DegreesToRadians(lng1);
        double lat2Rad = DegreesToRadians(lat2);
        double lng2Rad = DegreesToRadians(lng2);

        // Diferença entre as coordenadas
        double dLat = lat2Rad - lat1Rad;
        double dLng = lng2Rad - lng1Rad;

        // Fórmula de Haversine
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                   Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        // Distância em quilômetros
        return R * c;
    }

    private double DegreesToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
    }
>>>>>>> 7b148c2b8ae7f452586616b92a970a41fd43347c
}