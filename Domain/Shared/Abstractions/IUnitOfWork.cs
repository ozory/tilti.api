using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstractions;
using Domain.Features.Orders.Repository;
using Domain.Features.Plans.Repository;
using Domain.Features.Subscriptions.Repository;
using Domain.Features.Users.Repository;

namespace Domain.Shared.Abstractions;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IOrderRepository OrderRepository { get; }
    IPlanRepository PlanRepository { get; }
    ISubscriptionRepository SubscriptionRepository { get; }

    Task<bool> CommitAsync(CancellationToken cancellationToken);
}
