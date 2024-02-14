using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstractions;

namespace Domain.Features.Orders.Repository;

public interface IOrderRepository : IGenericRepository<Entities.Order>
{
    Task<IReadOnlyList<Entities.Order?>> GetOrdersByUser(long idUser);
    Task<IReadOnlyList<Entities.Order?>> GetOrdersByDriver(long idDriver);

}