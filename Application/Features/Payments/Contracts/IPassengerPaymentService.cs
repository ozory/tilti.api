namespace Application.Features.Payments.Contracts;

/// <summary>
/// Interface for passenger payment service operations
/// </summary>
public interface IPassengerPaymentService
{
    /// <summary>
    /// Create or retrieve existing wallet for passenger in Asaas
    /// </summary>
    Task<string> CreateOrGetWalletAsync(long userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a payment link in Asaas for passenger ride
    /// </summary>
    Task<(string paymentId, string pixLink, string qrCode)> CreatePaymentAsync(
        long orderId,
        decimal amount,
        string customerWalletId,
        DateTime dueDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check payment status in Asaas
    /// </summary>
    Task<bool> CheckPaymentStatusAsync(string asaasPaymentId, CancellationToken cancellationToken = default);
}
