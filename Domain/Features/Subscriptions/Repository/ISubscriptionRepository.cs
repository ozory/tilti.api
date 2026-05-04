using Domain.Abstractions;
using Domain.Features.Subscriptions.Entities;
using Domain.Subscriptions.Enums;

namespace Domain.Features.Subscriptions.Repository;

public interface ISubscriptionRepository : IGenericRepository<Subscription>
{
    Task<Subscription?> GetSubscriptionByUser(long idUser);
    Task<Subscription?> GetActiveSubscriptionByUser(long idUser);
    Task<bool> HasActiveSubscription(long idUser);
    Task<Subscription?> GetByAsaasPaymentId(string asaasPaymentId);
    Task<Subscription?> GetByAsaasSubscriptionId(string asaasSubscriptionId);
    Task<Subscription?> GetActiveSubscriptionByUserAndType(long userId, SubscriptionType subscriptionType); // NEW: For unified model
}
