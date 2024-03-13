using Domain.Abstractions;
using Domain.Features.Orders.Entities;

namespace Domain.Features.Orders.Repository;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<IReadOnlyList<Entities.Order?>> GetOpenedOrdersByUser(long idUser);
}