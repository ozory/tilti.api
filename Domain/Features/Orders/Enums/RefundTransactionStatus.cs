namespace Domain.Orders.Enums;

/// <summary>
/// Status of a refund transaction
/// </summary>
public enum RefundTransactionStatus : ushort
{
    Pending = 1,
    Completed = 2,
    Failed = 3
}