using Domain.Features.Users.Entities;
using Domain.Features.Users.Repository;
using Infrastructure.Data.Postgreesql.Shared;

namespace Infrastructure.Data.Postgreesql.Features.Users.Repository;

public class RateRepository :
    GenericRepository<Rate>,
    IRateRepository
{
    public RateRepository(TILTContext context) : base(context) { }
}
