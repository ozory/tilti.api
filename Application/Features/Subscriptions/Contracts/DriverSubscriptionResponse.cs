using Application.Features.Plans.Contracts;
using Application.Features.Users.Contracts;
using Domain.Features.Subscriptions.Entities;

namespace Application.Features.Subscriptions.Contracts;

/// <summary>
/// Response for driver subscription
/// </summary>
public record DriverSubscriptionResponse
(
    long Id,
    string Status,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime DueDate,
    string? PaymentLink,
    string? AsaasPaymentId,
    DateTime? PaidAt,
    PlanResponse Plan,
    UserResponse User
)
{
    public static implicit operator DriverSubscriptionResponse(DriverSubscription subscription)
        => new DriverSubscriptionResponse(
            subscription.Id,
            subscription.Status.ToString(),
            subscription.CreatedAt,
            subscription.UpdatedAt,
            subscription.DueDate,
            subscription.AsaasPaymentLink,
            subscription.AsaasPaymentId,
            subscription.PaidAt,
            (PlanResponse)subscription.Plan,
            (UserResponse)subscription.User
        );
}