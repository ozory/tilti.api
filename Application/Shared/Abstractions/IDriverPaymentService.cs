using Domain.Features.Subscriptions.Entities;
using Domain.Subscriptions.Enums;

namespace Application.Shared.Abstractions;

public interface ISubscriptionPaymentService
{
    Task<string> CreatePaymentLinkAsync(Subscription subscription, SubscriptionType subscriptionType, CancellationToken cancellationToken = default);
    Task<(string subscriptionId, string paymentLink)> CreateSubscriptionAsync(Subscription subscription, SubscriptionType subscriptionType, CancellationToken cancellationToken = default);
}
