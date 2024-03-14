using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payments.Entities;
using Domain.Features.Users.Entities;

namespace Application.Shared.Abstractions;

public interface IPaymentServices
{
    Task<User> CreateUser(long idUser, CancellationToken? cancellationToken);
    Task<Payment> CreateToken(long idUser, CancellationToken? cancellationToken);
}
