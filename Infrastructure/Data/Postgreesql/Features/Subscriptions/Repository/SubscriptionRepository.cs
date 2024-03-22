using Domain.Features.Subscriptions.Entities;
using Domain.Features.Subscriptions.Repository;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data.Postgreesql.Shared;
using Domain.Features.Users.Entities;
using Domain.Features.Plans.Entities;

namespace Infrastructure.Data.Postgreesql.Features.Subscriptions.Repository;

public class SubscriptionRepository :
    GenericRepository<Subscription>,
    ISubscriptionRepository
{

    private readonly string IncludeProperties = $"{nameof(User)},{nameof(Plan)}";

    public SubscriptionRepository(TILTContext context) : base(context) { }

    public override async Task<IReadOnlyList<Subscription>> GetAllAsync()
        => await Filter(includeProperties: IncludeProperties);

    public override async Task<Subscription?> GetByIdAsync(long id)
        => await FirstOrDefault(filter: s => s.Id == id, includeProperties: IncludeProperties);

    public async Task<Subscription?> GetSubscriptionByUser(long idUser)
        => await FirstOrDefault(filter: s => s.UserId == idUser, includeProperties: IncludeProperties);
}
