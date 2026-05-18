using Domain.Abstractions;
using Domain.Orders.Enums;

namespace Domain.Features.Orders.Entities;

/// <summary>
/// Represents a PIX refund transaction for a canceled order
/// </summary>
public class RefundTransaction : Entity
{
    #region PROPERTIES

    public Guid TransactionId { get; protected set; }
    public long OrderId { get; protected set; }
    public decimal Amount { get; protected set; }
    public string CustomerWalletId { get; protected set; } = null!;
    public RefundTransactionStatus Status { get; protected set; }
    public int RetryCount { get; protected set; }
    public string? ErrorDetails { get; protected set; }
    public string? AsaasTransferId { get; protected set; }

    #endregion

    #region CONSTRUCTORS

    public RefundTransaction() { }

    public static RefundTransaction Create(
        long orderId,
        decimal amount,
        string customerWalletId)
    {
        return new RefundTransaction
        {
            TransactionId = Guid.NewGuid(),
            OrderId = orderId,
            Amount = amount,
            CustomerWalletId = customerWalletId,
            Status = RefundTransactionStatus.Pending,
            RetryCount = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    #endregion

    #region METHODS

    public void SetCompleted(string asaasTransferId)
    {
        Status = RefundTransactionStatus.Completed;
        AsaasTransferId = asaasTransferId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetFailed(string errorDetails, int retryCount)
    {
        Status = RefundTransactionStatus.Failed;
        ErrorDetails = errorDetails;
        RetryCount = retryCount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void IncrementRetryCount()
    {
        RetryCount++;
        UpdatedAt = DateTime.UtcNow;
    }

    #endregion
}