using Domain.Features.Orders.Entities;
using Domain.Features.Orders.Repository;
using Domain.Orders.Enums;
using Infrastructure.Data.Postgreesql.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Postgreesql.Features.Orders.Repository;

public class RefundTransactionRepository :
    GenericRepository<RefundTransaction>,
    IRefundTransactionRepository
{
    public RefundTransactionRepository(TILTContext context) : base(context)
    {
    }

    public override async Task<RefundTransaction> SaveAsync(RefundTransaction transaction)
    {
        await Entities.AddAsync(transaction);
        return transaction;
    }

    public async Task<RefundTransaction?> GetByIdAsync(Guid transactionId)
    {
        return await Entities.FirstOrDefaultAsync(t => t.TransactionId == transactionId);
    }

    public async Task<RefundTransaction?> GetByOrderIdAsync(long orderId)
    {
        return await Entities.FirstOrDefaultAsync(t => t.OrderId == orderId);
    }

    public override async Task<RefundTransaction> UpdateAsync(RefundTransaction transaction)
    {
        Entities.Attach(transaction);
        _context.Entry(transaction).State = EntityState.Modified;
        return transaction;
    }

    public async Task<IReadOnlyList<RefundTransaction>> GetPendingTransactionsAsync()
    {
        return await Entities
            .Where(t => t.Status == RefundTransactionStatus.Pending)
            .AsNoTracking()
            .ToListAsync();
    }
}