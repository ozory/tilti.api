using Domain.Enums;
using Infrastructure.Data.Postgreesql.Features.Plans.Entities;
using Infrastructure.Data.Postgreesql.Features.Plans.Maps;
using Infrastructure.Data.Postgreesql.Features.Subscriptions.Entities;
using Infrastructure.Data.Postgreesql.Features.Users.Entities;
using Infrastructure.Data.Postgreesql.Features.Users.Maps;
using DomainSubscription = Domain.Features.Subscriptions.Entities.Subscription;

namespace Infrastructure.Data.Postgreesql.Features.Subscriptions.Maps;

public static class SubscriptionMaps
{
    internal static Subscription ToPersistenceSub(this DomainSubscription subscription)
    {
        Subscription persistenceSub = new Subscription
        {
            Id = subscription.Id,
            UserId = subscription.User.Id,
            PlanId = subscription.Plan.Id,
            DueDate = subscription.DueDate,
            Created = subscription.CreatedAt,
            Updated = subscription.UpdatedAt
        };

        return persistenceSub;
    }

    internal static DomainSubscription ToDomainSub(this Subscription subscription)
    {
        var domainSub = DomainSubscription.Create(
            subscription.Id,
            subscription.User.ToDomainUser(),
            subscription.Plan.ToDomainPlan()
        );

        domainSub.SetUpdatedAt(subscription.Updated);

        return domainSub;
    }
}
