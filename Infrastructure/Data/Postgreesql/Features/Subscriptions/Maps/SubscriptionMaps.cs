using Domain.Subscriptions.Enums;
using Infrastructure.Data.Postgreesql.Features.Plans.Maps;
using Infrastructure.Data.Postgreesql.Features.Subscriptions.Entities;
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
            Status = (ushort)subscription.Status,
            Created = subscription.CreatedAt,
            Updated = subscription.UpdatedAt
        };

        return persistenceSub;
    }

    internal static DomainSubscription ToDomainSub(this Subscription subscription)
    {
        var domainSub = DomainSubscription.Create(
            subscription.Id,
            subscription.User,
            subscription.Plan.ToDomainPlan(),
            subscription.Created
        );

        domainSub.SetDueDate(subscription.DueDate);
        domainSub.SetStatus((SubscriptionStatus)subscription.Status);
        domainSub.SetUpdatedAt(subscription.Updated);

        return domainSub;
    }
}
