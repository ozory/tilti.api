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
    IUserRepository GetUserRepository();
    IOrderRepository GetOrderRepository();
    ISubscriptionRepository GetSubscriptionRepository();
    IPlanRepository GetPlanRepository();

    Task CommitAsync(CancellationToken cancellationToken);
}
