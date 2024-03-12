using Domain.Features.Subscriptions.Entities;
using Domain.Features.Subscriptions.Repository;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data.Postgreesql.Shared;

namespace Infrastructure.Data.Postgreesql.Features.Subscriptions.Repository;

public class SubscriptionRepository :
    GenericRepository<Subscription>,
    ISubscriptionRepository
{

    public SubscriptionRepository(TILTContext context) : base(context) { }

    public override async Task<IReadOnlyList<Subscription>> GetAllAsync()
    {
        return await _context.Subscriptions
            .Include(u => u.User)
            .Include(u => u.Plan)
            .AsNoTracking()
            .ToListAsync();
    }

    public override async Task<Subscription?> GetByIdAsync(long id)
    {
        return await _context.Subscriptions
            .Include(u => u.User)
            .Include(p => p.Plan)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Subscription?> GetSubscriptionByUser(long idUser)
    {
        return await _context.Subscriptions
            .Include(u => u.User)
            .Include(p => p.Plan)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == idUser);
    }

}
