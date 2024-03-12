using Domain.Features.Users.Entities;
using Domain.Features.Users.Repository;
using Infrastructure.Data.Postgreesql.Shared;
using DomainUser = Domain.Features.Users.Entities.User;

namespace Infrastructure.Data.Postgreesql.Features.Users.Repository;

public class UserRepository :
    GenericRepository<User>,
    IUserRepository
{
    public UserRepository(TILTContext context) : base(context) { }

    public async Task<DomainUser?> GetByEmail(string email) => await FirstOrDefault(u => u.Email.Value == email);

    public async Task<DomainUser?> GetByDocument(string document) => await FirstOrDefault(u => u.Document.Value == document);
}
