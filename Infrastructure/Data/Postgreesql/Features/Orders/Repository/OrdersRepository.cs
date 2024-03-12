using System.Collections.Immutable;
using Domain.Enums;
using Domain.Features.Orders.Entities;
using Domain.Features.Orders.Repository;
using Infrastructure.Data.Postgreesql.Shared;
using Microsoft.EntityFrameworkCore;
using DomainOrder = Domain.Features.Orders.Entities.Order;

namespace Infrastructure.Data.Postgreesql.Features.Orders.Repository;

public class OrdersRepository :
    GenericRepository<Order>,
    IOrderRepository
{
    public OrdersRepository(TILTContext context) : base(context) { }

    public async Task<IReadOnlyList<Order?>> GetOrdersByDriver(long idDriver) => await Filter(o => o.DriverId == idDriver);

    public async Task<IReadOnlyList<Order?>> GetOrdersByUser(long idUser) => await Filter(o => o.UserId == idUser);

    public async Task<IReadOnlyList<Order?>> GetOpenedOrdersByUser(long idUser)
    {
        var status = Enum.GetValues(typeof(OrderStatus)).Cast<ushort>().ToList();

        var orders = await _context.Orders.AsNoTracking()
            .Where(u => u.UserId == idUser && status.Contains((ushort)u.Status)).ToListAsync();
        var orderLists = orders.Select(u => (DomainOrder)u).ToImmutableList();

        return orderLists!;
    }

}