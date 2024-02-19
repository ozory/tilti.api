using System.Collections.Immutable;
using Domain.Features.Orders.Entities;
using Domain.Features.Orders.Repository;
using Infrastructure.Data.Postgreesql.Features.Subscriptions.Maps;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Postgreesql.Features.Orders.Repository;

public class OrdersRepository : IOrderRepository
{
    private readonly TILTContext _context;

    public OrdersRepository(TILTContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Order>> GetAllAsync()
    {
        var orders = await _context.Orders.AsNoTracking().ToListAsync();
        var orderLists = orders.Select(u => u.ToDomainOrder()).ToImmutableList();

        return orderLists!;
    }

    public async Task<Order?> GetByIdAsync(long id)
    {
        var order = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        return order?.ToDomainOrder() ?? null;
    }

    public async Task<IReadOnlyList<Order?>> GetOrdersByDriver(long idDriver)
    {
        var orders = await _context.Orders.AsNoTracking().Where(u => u.DriverId == idDriver).ToListAsync();
        var orderLists = orders.Select(u => u.ToDomainOrder()).ToImmutableList();

        return orderLists!;
    }

    public async Task<IReadOnlyList<Order?>> GetOrdersByUser(long idUser)
    {
        var orders = await _context.Orders.AsNoTracking().Where(u => u.UserId == idUser).ToListAsync();
        var orderLists = orders.Select(u => u.ToDomainOrder()).ToImmutableList();

        return orderLists!;
    }

    public async Task<IReadOnlyList<Order?>> GetOpenedOrdersByUser(long idUser)
    {
        int[] status = [1, 2, 3];

        // Pending
        // Accepted
        // InTransit

        var orders = await _context.Orders.AsNoTracking()
            .Where(u => u.UserId == idUser && status.Contains(u.Status)).ToListAsync();
        var orderLists = orders.Select(u => u.ToDomainOrder()).ToImmutableList();

        return orderLists!;
    }

    public async Task<Order> SaveAsync(Order entity)
    {
        var order = entity.ToPersistenceOrder();
        _context.Add(order);
        await _context.SaveChangesAsync();
        return order?.ToDomainOrder()!;
    }

    public async Task<Order> UpdateAsync(Order entity)
    {
        var order = entity.ToPersistenceOrder();
        _context.Update(order);
        await _context.SaveChangesAsync();
        return order?.ToDomainOrder()!;
    }
}