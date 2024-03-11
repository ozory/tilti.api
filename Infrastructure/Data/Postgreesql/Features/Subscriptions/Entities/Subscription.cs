using Domain.Subscriptions.Enums;
using Infrastructure.Data.Postgreesql.Features.Plans.Entities;
using Infrastructure.Data.Postgreesql.Features.Users.Entities;
using Infrastructure.Data.Postgreesql.Shared.Abstractions;

using DomainSubscription = Domain.Features.Subscriptions.Entities.Subscription;
namespace Infrastructure.Data.Postgreesql.Features.Subscriptions.Entities;

public class Subscription : InfrastructureEntity
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long PlanId { get; set; }
    public ushort Status { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; } = DateTime.Now;

    public User User { get; set; } = null!;
    public Plan Plan { get; set; } = null!;

    public static implicit operator Subscription(DomainSubscription subscription)
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

    public static explicit operator DomainSubscription(Subscription? subscription)
    {
        if (subscription is null) return null!;
        var domainSub = DomainSubscription.Create(
            subscription.Id,
            subscription.User,
            subscription.Plan,
            subscription.Created
        );

        domainSub.SetDueDate(subscription.DueDate);
        domainSub.SetStatus((SubscriptionStatus)subscription.Status);
        domainSub.SetUpdatedAt(subscription.Updated);

        return domainSub;
    }
}
