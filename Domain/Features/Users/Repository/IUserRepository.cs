using Domain.Abstractions;
using Domain.Features.Users.Entities;

namespace Domain.Features.Users.Repository;

public interface IUserRepository : IGenericRepository<User>
{
}
