using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Subscription.Entities;
using Domain.Features.Subscription.Repository;
using Infrastructure.Data.Postgreesql;
using Infrastructure.Data.Postgreesql.Shared;

namespace Infrastructure.Data.Postgreesql.Features.Subscriptions.Repository;

public class SubscriptionRepository : GenericRepository<Subscription>, ISubscriptionRepository
{
    public SubscriptionRepository(TILTContext context)
        : base(context)
    {
    }
}
