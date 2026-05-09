namespace Application.Features.Payments.Contracts;

/// <summary>
/// Interface for driver transfer service operations
/// Handles transfer of funds from Tilt wallet to driver wallet
/// </summary>
public interface IDriverTransferService
{
    /// <summary>
    /// Transfer funds from Tilt wallet to driver wallet
    /// </summary>
    /// <param name="orderId">Order ID for reference</param>
    /// <param name="amount">Amount to transfer</param>
    /// <param name="driverWalletId">Driver's Asaas wallet ID</param>
    /// <returns>Transfer ID from Asaas</returns>
    Task<string> TransferToDriverAsync(
        long orderId,
        decimal amount,
        string driverWalletId,
        CancellationToken cancellationToken = default);
}