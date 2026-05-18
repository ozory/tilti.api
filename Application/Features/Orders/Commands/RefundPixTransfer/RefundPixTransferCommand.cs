using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Orders.Commands.RefundPixTransfer;

/// <summary>
/// Command to initiate a PIX refund transfer for a canceled order
/// </summary>
public sealed record RefundPixTransferCommand(
    long OrderId,
    decimal RefundAmount,
    string CustomerWalletId
) : ICommand<Result<RefundPixTransferResponse>>;

/// <summary>
/// Response for PIX refund transfer command
/// </summary>
public record RefundPixTransferResponse(
    Guid TransactionId,
    long OrderId,
    decimal Amount,
    string CustomerWalletId,
    string Status,
    string? AsaasTransferId = null
);