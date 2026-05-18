
namespace Infrastructure.External.Features.Payments.Contracts;

/// <summary>
/// Request to transfer funds via PIX in Asaas
/// </summary>
internal record PixTransferRequest(
    string value,
    string? pixTransferId,
    string? walletId,
    string? externalReference
);
