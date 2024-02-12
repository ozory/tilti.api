using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Plans.Contracts;
using Application.Features.Users.Contracts;
using Domain.Features.Subscriptions.Entities;

namespace Application.Features.Subscriptions.Contracts;

public record SubscriptionResponse
(
    long Id,
    string Status,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime DueDate,
    PlanResponse plan,
    UserResponse user
)
{
    public static implicit operator SubscriptionResponse(Subscription subscription)
        => new SubscriptionResponse(
                subscription.Id,
                subscription.Status.ToString(),
                subscription.CreatedAt,
                subscription.UpdatedAt,
                subscription.DueDate,
                (PlanResponse)subscription.Plan,
                (UserResponse)subscription.User
                );
}

