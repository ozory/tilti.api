

using Application.Shared.Abstractions;
using Domain.Features.Payments.Entities;
using Domain.Features.Users.Entities;

namespace Infrastructure.Features.Orders.Services;

public class PaymentServices : IPaymentServices
{
    public Task<Payment> CreatePaymentToken(User user)
    {
        throw new NotImplementedException();
    }
}