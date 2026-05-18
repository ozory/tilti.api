using System.Threading;
using System.Threading.Tasks;

namespace Application.Shared.Abstractions;

/// <summary>
/// Service for transferring funds via PIX
/// </summary>
public interface IPixTransferService
{
    /// <summary>
    /// Transfer refund amount from Tilt wallet to customer wallet via PIX
    /// </summary>
    Task<PixTransferResponse> TransferRefundAsync(
        long orderId,
        decimal amount,
        string customerWalletId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Response from PIX transfer creation
/// </summary>
public record PixTransferResponse(
    string id,
    string status,
    decimal value,
    string? pixTransferId,
    string? walletId,
    DateTime dateCreated,
    string? externalReference
);