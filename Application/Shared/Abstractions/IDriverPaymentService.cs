using Domain.Features.Subscriptions.Entities;

namespace Application.Shared.Abstractions;

/// <summary>
/// Service interface for driver payment operations with Asaas
/// </summary>
public interface IDriverPaymentService
{
    /// <summary>
    /// Creates a subscription in Asaas for driver
    /// </summary>
    /// <param name="driverSubscription">The driver subscription to create payment for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Tuple with subscription ID and payment link URL from Asaas</returns>
    Task<(string subscriptionId, string paymentLink)> CreateSubscriptionAsync(DriverSubscription driverSubscription, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a payment link for driver subscription (legacy - for single payments)
    /// </summary>
    /// <param name="driverSubscription">The driver subscription to create payment for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Payment link URL from Asaas</returns>
    Task<string> CreatePaymentLinkAsync(DriverSubscription driverSubscription, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks the payment status from Asaas
    /// </summary>
    /// <param name="asaasPaymentId">The Asaas payment ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if payment is approved</returns>
    Task<bool> CheckPaymentStatusAsync(string asaasPaymentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a wallet for the driver in Asaas
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Wallet ID from Asaas</returns>
    Task<string> CreateDriverWalletAsync(long userId, CancellationToken cancellationToken = default);
}