using Domain.Features.Subscriptions.Entities;
using Domain.Features.Subscriptions.Repository;
using Domain.Subscriptions.Enums;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data.Postgreesql.Shared;

namespace Infrastructure.Data.Postgreesql.Features.Subscriptions.Repository;

/// <summary>
/// Repository for DriverSubscription entity
/// </summary>
public class DriverSubscriptionRepository :
    GenericRepository<DriverSubscription>,
    IDriverSubscriptionRepository
{

    private readonly string IncludeProperties = $"{nameof(Domain.Features.Users.Entities.User)},{nameof(Domain.Features.Plans.Entities.Plan)}";

    public DriverSubscriptionRepository(TILTContext context) : base(context) { }

    public override async Task<IReadOnlyList<DriverSubscription>> GetAllAsync()
        => await Filter(includeProperties: IncludeProperties);

    public override async Task<DriverSubscription?> GetByIdAsync(long id)
        => await FirstOrDefault(filter: s => s.Id == id, includeProperties: IncludeProperties);

    public async Task<DriverSubscription?> GetSubscriptionByUser(long idUser)
        => await FirstOrDefault(filter: s => s.UserId == idUser, includeProperties: IncludeProperties);

    public async Task<DriverSubscription?> GetActiveSubscriptionByUser(long idUser)
        => await FirstOrDefault(
            filter: s => s.UserId == idUser && s.Status == SubscriptionStatus.Active,
            includeProperties: IncludeProperties);

    public async Task<bool> HasActiveSubscription(long idUser)
    {
        var subscription = await GetActiveSubscriptionByUser(idUser);
        return subscription != null;
    }

    public async Task<DriverSubscription?> GetByAsaasPaymentId(string asaasPaymentId)
        => await FirstOrDefault(filter: s => s.AsaasPaymentId == asaasPaymentId, includeProperties: IncludeProperties);
}