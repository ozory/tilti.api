using Domain.Abstractions;
using Domain.Features.Subscriptions.Entities;


namespace Domain.Features.Subscriptions.Repository;

public interface ISubscriptionRepository : IGenericRepository<Subscription>
{
    Task<Subscription?> GetSubscriptionByUser(long idUser);
}
