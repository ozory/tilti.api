using System;

namespace Application.Features.Orders.Events;

/// <summary>
/// Event emitted when a PIX refund transfer completes successfully.
/// </summary>
public class RefundPixTransferCompletedEvent
{
    public Guid TransactionId { get; init; }
    public long OrderId { get; init; }
    public decimal Amount { get; init; }
    public string CustomerWalletId { get; init; }
    public DateTime Timestamp { get; init; }
    public string AsaasTransferId { get; init; }

    public RefundPixTransferCompletedEvent(
        Guid transactionId,
        long orderId,
        decimal amount,
        string customerWalletId,
        string asaasTransferId)
    {
        TransactionId = transactionId;
        OrderId = orderId;
        Amount = amount;
        CustomerWalletId = customerWalletId;
        AsaasTransferId = asaasTransferId;
        Timestamp = DateTime.UtcNow;
    }
}