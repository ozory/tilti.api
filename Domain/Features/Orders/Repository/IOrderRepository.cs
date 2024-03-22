using Domain.Abstractions;
using Domain.Features.Orders.Entities;
using NetTopologySuite.Geometries;

namespace Domain.Features.Orders.Repository;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<IReadOnlyList<Order?>> GetOpenedOrdersByUser(long idUser);
    Task<IReadOnlyList<Order?>> GetOpenedOrdersThatExpired(DateTime expireTime);
    Task<IReadOnlyList<Order?>> GetOrdersByPoint(Point point);
}