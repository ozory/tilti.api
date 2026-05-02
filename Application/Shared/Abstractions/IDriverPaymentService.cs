using Domain.Features.Subscriptions.Entities;

namespace Application.Shared.Abstractions;

public interface IDriverPaymentService
{
    Task<string> CreatePaymentLinkAsync(DriverSubscription driverSubscription, CancellationToken cancellationToken = default);
    Task<(string subscriptionId, string paymentLink)> CreateSubscriptionAsync(DriverSubscription driverSubscription, CancellationToken cancellationToken = default);
}
