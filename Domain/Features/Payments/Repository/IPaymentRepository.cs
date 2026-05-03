using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payments.Entities;

namespace Domain.Features.Payments.Repository;

public interface IPaymentRepository
{
    Task<Payment?> GetByAsaasPaymentId(string asaasPaymentId);
    Task UpdateAsync(Payment payment);
}
