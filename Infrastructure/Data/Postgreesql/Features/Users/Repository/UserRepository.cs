using System.Collections.Immutable;
using Domain.Features.Users.Entities;
using Domain.Features.Users.Repository;
using Infrastructure.Data.Postgreesql.Shared;
using Microsoft.EntityFrameworkCore;
using DomainUser = Domain.Features.Users.Entities.User;
using InfrastructureUser = Infrastructure.Data.Postgreesql.Features.Users.Entities.User;

namespace Infrastructure.Data.Postgreesql.Features.Users.Repository;

public class UserRepository :
    GenericRepository<DomainUser, InfrastructureUser>,
    IUserRepository
{
    public UserRepository(TILTContext context) : base(context) { }

    public async Task<DomainUser?> GetByDocument(string document)
    {
        var user = await this.dbSet.AsNoTracking().FirstOrDefaultAsync(u => u.Document == document);
        return (DomainUser?)user;
    }

    public async Task<DomainUser?> GetByEmail(string email)
    {
        var user = await this.dbSet.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
        return (DomainUser?)user;
    }
}
