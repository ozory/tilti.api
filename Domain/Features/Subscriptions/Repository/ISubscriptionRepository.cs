using Domain.Abstractions;
using DomainSubscription = Domain.Features.Subscriptions.Entities.Subscription;

namespace Domain.Features.Subscription.Repository;

public interface ISubscriptionRepository : IGenericRepository<DomainSubscription>
{
    Task<DomainSubscription?> GetSubscriptionByUser(long idUser);
}
