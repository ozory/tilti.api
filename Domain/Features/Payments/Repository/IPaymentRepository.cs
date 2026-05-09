using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payments.Entities;

namespace Domain.Features.Payments.Repository;

public interface IPaymentRepository
{
    Task<Payment?> GetByAsaasPaymentId(string asaasPaymentId);
    Task<Payment?> GetByOrderIdAsync(long orderId);
    Task<Payment> SaveAsync(Payment payment);
    Task UpdateAsync(Payment payment);
}
