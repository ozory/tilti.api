using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Subscriptions.Entities;
using Domain.Features.Subscription.Repository;
using Infrastructure.Data.Postgreesql;
using Infrastructure.Data.Postgreesql.Shared;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data.Postgreesql.Features.Subscriptions.Maps;
using System.Collections.Immutable;

namespace Infrastructure.Data.Postgreesql.Features.Subscriptions.Repository;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly TILTContext _context;

    public SubscriptionRepository(TILTContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Subscription>> GetAllAsync()
    {
        var subscriptions = await _context.Subscriptions
            .Include(u => u.User)
            .Include(u => u.Plan)
            .AsNoTracking()
            .ToListAsync();
        var list = subscriptions.Select(u => u.ToDomainSub()).ToImmutableList();

        return list!;
    }

    public async Task<Subscription?> GetByIdAsync(long id)
    {
        var subscriptions = await _context.Subscriptions.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        return subscriptions?.ToDomainSub() ?? null;
    }

    public async Task<Subscription?> GetSubscriptionByUser(long idUser)
    {
        var subscriptions = await _context.Subscriptions.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == idUser);
        return subscriptions?.ToDomainSub() ?? null;
    }

    public async Task<Subscription> SaveAsync(Subscription entity)
    {
        var plan = entity.ToPersistenceSub();
        _context.Add(plan);
        await _context.SaveChangesAsync();
        return plan?.ToDomainSub()!;
    }

    public async Task<Subscription> UpdateAsync(Subscription entity)
    {
        var plan = entity.ToPersistenceSub();
        _context.Update(plan);
        await _context.SaveChangesAsync();
        return plan?.ToDomainSub()!;
    }
}
