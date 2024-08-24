using Domain.Features.Orders.Entities;
using Domain.Features.Orders.Repository;
using Domain.Features.Users.Entities;
using Domain.Features.Users.Repository;
using Infrastructure.Data.Postgreesql.Shared;

namespace Infrastructure.Data.Postgreesql.Features.Orders.Repository;

public class RateRepository :
    GenericRepository<Rate>,
    IRateRepository
{
    public RateRepository(TILTContext context) : base(context) { }
}
