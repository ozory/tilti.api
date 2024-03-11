using Domain.Features.Subscriptions.Entities;
using Domain.Features.Subscriptions.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using Domain.Abstractions;
using InfrastructureSubscription = Infrastructure.Data.Postgreesql.Features.Subscriptions.Entities.Subscription;
using Infrastructure.Data.Postgreesql.Shared;

namespace Infrastructure.Data.Postgreesql.Features.Subscriptions.Repository;

public class SubscriptionRepository :
    GenericRepository<InfrastructureSubscription>,
    ISubscriptionRepository
{

    public SubscriptionRepository(TILTContext context) : base(context) { }

    public new async Task<IReadOnlyList<Subscription>> GetAllAsync()
    {
        var subscriptions = await _context.Subscriptions
            .Include(u => u.User)
            .Include(u => u.Plan)
            .AsNoTracking()
            .ToListAsync();
        var list = subscriptions.Select(u => (Subscription)u).ToImmutableList();

        return list!;
    }

    public new async Task<Subscription?> GetByIdAsync(long id)
    {
        var subscriptions = await _context.Subscriptions
            .Include(u => u.User)
            .Include(p => p.Plan)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
        return (Subscription?)subscriptions;
    }

    public async Task<Subscription?> GetSubscriptionByUser(long idUser)
    {
        var subscriptions = await _context.Subscriptions
            .Include(u => u.User)
            .Include(p => p.Plan)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == idUser);
        return (Subscription?)subscriptions;
    }

    public async Task<Subscription> SaveAsync(Subscription entity)
        => (Subscription)await base.SaveAsync((InfrastructureSubscription)entity);

    public async Task<Subscription> UpdateAsync(Subscription entity)
        => (Subscription)await base.UpdateAsync((InfrastructureSubscription)entity);

}
