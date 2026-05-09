using Application.Features.Plans.Contracts;
using Application.Features.Users.Contracts;
using Domain.Features.Subscriptions.Entities;

namespace Application.Features.Subscriptions.Contracts;

/// <summary>
/// Response for subscription (driver or passenger)
/// </summary>
public record SubscriptionResponse
(
    long Id,
    string Status,
    string SubscriptionType, // NEW: Driver or Passenger
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime DueDate,
    string? PaymentLink,
    string? AsaasPaymentId,
    string? AsaasSubscriptionId,
    DateTime? PaidAt,
    PlanResponse Plan,
    UserResponse User
)
{
    public static implicit operator SubscriptionResponse(Subscription subscription)
        => new SubscriptionResponse(
            subscription.Id,
            subscription.Status.ToString(),
            subscription.SubscriptionType.ToString(), // NEW
            subscription.CreatedAt,
            subscription.UpdatedAt,
            subscription.DueDate,
            subscription.AsaasPaymentLink,
            subscription.AsaasPaymentId,
            subscription.AsaasSubscriptionId,
            subscription.PaidAt,
            (PlanResponse)subscription.Plan,
            (UserResponse)subscription.User
        );
}