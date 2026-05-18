using Domain.Features.Orders.Entities;

namespace Domain.Features.Orders.Repository;

/// <summary>
/// Repository for RefundTransaction entities stored in PostgreSQL
/// </summary>
public interface IRefundTransactionRepository
{
    Task<RefundTransaction> SaveAsync(RefundTransaction transaction);
    Task<RefundTransaction?> GetByIdAsync(Guid transactionId);
    Task<RefundTransaction?> GetByOrderIdAsync(long orderId);
    Task<RefundTransaction> UpdateAsync(RefundTransaction transaction);
    Task<IReadOnlyList<RefundTransaction>> GetPendingTransactionsAsync();
}