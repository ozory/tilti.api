using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstractions;
using Domain.Entities;
using DomainSubscription = Domain.Features.Subscription.Entities.Subscription;

namespace Domain.Features.Subscription.Repository;

public interface ISubscriptionRepository : IGenericRepository<DomainSubscription>
{

}
