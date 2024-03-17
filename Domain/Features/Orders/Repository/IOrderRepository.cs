using Domain.Abstractions;
using Domain.Features.Orders.Entities;
using Domain.Shared.ValueObjects;
using NetTopologySuite.Geometries;

namespace Domain.Features.Orders.Repository;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<IReadOnlyList<Entities.Order?>> GetOpenedOrdersByUser(long idUser);
    Task<IReadOnlyList<Entities.Order?>> GetOrdersByPoint(Point point);
}