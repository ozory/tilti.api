using System.Collections.Immutable;
using Domain.Enums;
using Domain.Features.Orders.Entities;
using Domain.Features.Orders.Repository;
using Infrastructure.Data.Postgreesql.Shared;
using Microsoft.EntityFrameworkCore;
using DomainOrder = Domain.Features.Orders.Entities.Order;
using InfrastructureOrder = Infrastructure.Data.Postgreesql.Features.Orders.Entities.Order;

namespace Infrastructure.Data.Postgreesql.Features.Orders.Repository;

public class OrdersRepository :
    GenericRepository<InfrastructureOrder>,
    IOrderRepository
{
    public OrdersRepository(TILTContext context) : base(context) { }

    public async Task<DomainOrder> SaveAsync(DomainOrder entity)
         => (DomainOrder)await base.SaveAsync((InfrastructureOrder)entity);

    public async Task<DomainOrder> UpdateAsync(DomainOrder entity)
        => (DomainOrder)await base.UpdateAsync((InfrastructureOrder)entity);

    public new async Task<DomainOrder?> GetByIdAsync(long id)
        => (DomainOrder)await base.GetByIdAsync(id);

    public new async Task<IReadOnlyList<DomainOrder>> GetAllAsync()
        => (IReadOnlyList<DomainOrder>)await base.GetAllAsync();

    public async Task<IReadOnlyList<Order?>> GetOrdersByDriver(long idDriver)
    {
        var orders = await _context.Orders.AsNoTracking().Where(u => u.DriverId == idDriver).ToListAsync();
        var orderLists = orders.Select(u => (DomainOrder)u).ToImmutableList();

        return orderLists!;
    }

    public async Task<IReadOnlyList<Order?>> GetOrdersByUser(long idUser)
    {
        var orders = await _context.Orders.AsNoTracking().Where(u => u.UserId == idUser).ToListAsync();
        var orderLists = orders.Select(u => (DomainOrder)u).ToImmutableList();

        return orderLists!;
    }

    public async Task<IReadOnlyList<Order?>> GetOpenedOrdersByUser(long idUser)
    {
        var status = Enum.GetValues(typeof(OrderStatus)).Cast<ushort>().ToList();
        // Pending
        // Accepted
        // InTransit

        var orders = await _context.Orders.AsNoTracking()
            .Where(u => u.UserId == idUser && status.Contains(u.Status)).ToListAsync();
        var orderLists = orders.Select(u => (DomainOrder)u).ToImmutableList();

        return orderLists!;
    }

}