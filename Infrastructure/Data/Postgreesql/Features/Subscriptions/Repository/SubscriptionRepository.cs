using Domain.Features.Subscriptions.Entities;
using Domain.Features.Subscriptions.Repository;
using Domain.Subscriptions.Enums;
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

    public async Task<Subscription?> GetActiveSubscriptionByUser(long idUser)
        => await FirstOrDefault(
            filter: s => s.UserId == idUser && s.Status == SubscriptionStatus.Active,
            includeProperties: IncludeProperties);

    public async Task<bool> HasActiveSubscription(long idUser)
        => await Entities.CountAsync(s => s.UserId == idUser && s.Status == SubscriptionStatus.Active) > 0;

    public async Task<Subscription?> GetByAsaasPaymentId(string asaasPaymentId)
        => await FirstOrDefault(
            filter: s => s.AsaasPaymentId == asaasPaymentId,
            includeProperties: IncludeProperties);

    public async Task<Subscription?> GetByAsaasSubscriptionId(string asaasSubscriptionId)
        => await FirstOrDefault(
            filter: s => s.AsaasSubscriptionId == asaasSubscriptionId,
            includeProperties: IncludeProperties);

    // NEW: For unified model - get active subscription by user and type
    public async Task<Subscription?> GetActiveSubscriptionByUserAndType(long userId, SubscriptionType subscriptionType)
        => await FirstOrDefault(
            filter: s => s.UserId == userId &&
                       s.SubscriptionType == subscriptionType &&
                       (s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.PendingApproval),
            includeProperties: IncludeProperties);
}
