using Domain.Features.Payments.Entities;
using Domain.Features.Payments.Repository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Postgreesql.Features.Payments.Repository;

/// <summary>
/// Repository implementation for Payment entity
/// </summary>
public class PaymentRepository : IPaymentRepository
{
    private readonly TILTContext _context;

    public PaymentRepository(TILTContext context)
    {
        _context = context;
    }

    public async Task<Payment?> GetByAsaasPaymentId(string asaasPaymentId)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(p => p.AsaasPaymentId == asaasPaymentId);
    }

    public async Task<Payment?> GetByOrderIdAsync(long orderId)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(p => p.OrderId == orderId);
    }

    public async Task<Payment> SaveAsync(Payment payment)
    {
        if (payment.Id == 0)
        {
            await _context.Payments.AddAsync(payment);
        }
        else
        {
            _context.Payments.Update(payment);
        }

        return payment;
    }

    public async Task UpdateAsync(Payment payment)
    {
        _context.Payments.Update(payment);
        await Task.CompletedTask;
    }
}
