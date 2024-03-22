using Domain.Features.Users.Entities;
using Domain.Features.Users.Repository;
using Infrastructure.Data.Postgreesql.Shared;

namespace Infrastructure.Data.Postgreesql.Features.Users.Repository;

public class UserRepository :
    GenericRepository<User>,
    IUserRepository
{
    public UserRepository(TILTContext context) : base(context) { }
}
