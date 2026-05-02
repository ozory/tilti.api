using Domain.Abstractions;
using Domain.Features.Subscriptions.Entities;

namespace Domain.Features.Subscriptions.Repository;

public interface IDriverSubscriptionRepository : IGenericRepository<DriverSubscription>
{
    Task<DriverSubscription?> GetSubscriptionByUser(long idUser);
    Task<DriverSubscription?> GetActiveSubscriptionByUser(long idUser);
    Task<bool> HasActiveSubscription(long idUser);
    Task<DriverSubscription?> GetByAsaasPaymentId(string asaasPaymentId);
    Task<DriverSubscription?> GetByAsaasSubscriptionId(string asaasSubscriptionId);
}