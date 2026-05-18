using System;

namespace Application.Features.Orders.Events;

/// <summary>
/// Event emitted when a PIX refund transfer fails after all retries are exhausted.
/// </summary>
public class RefundPixTransferFailedEvent
{
    public Guid TransactionId { get; init; }
    public long OrderId { get; init; }
    public string ErrorDetails { get; init; }
    public int RetryCount { get; init; }
    public DateTime Timestamp { get; init; }

    public RefundPixTransferFailedEvent(
        Guid transactionId,
        long orderId,
        string errorDetails,
        int retryCount)
    {
        TransactionId = transactionId;
        OrderId = orderId;
        ErrorDetails = errorDetails;
        RetryCount = retryCount;
        Timestamp = DateTime.UtcNow;
    }
}