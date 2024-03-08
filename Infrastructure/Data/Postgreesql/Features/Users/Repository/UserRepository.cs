using System.Collections.Immutable;
using Domain.Abstractions;
using Domain.Features.Users.Entities;
using Domain.Features.Users.Repository;
using Infrastructure.Data.Postgreesql.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DomainUser = Domain.Features.Users.Entities.User;
using InfrastructureUser = Infrastructure.Data.Postgreesql.Features.Users.Entities.User;

namespace Infrastructure.Data.Postgreesql.Features.Users.Repository;

public class UserRepository :
    GenericRepository<InfrastructureUser>,
    IUserRepository
{
    public UserRepository(TILTContext context, IMediator mediator) : base(context, mediator) { }

    public async Task<DomainUser> SaveAsync(DomainUser entity)
        => (DomainUser)await base.SaveAsync((InfrastructureUser)entity);

    public async Task<DomainUser> UpdateAsync(DomainUser entity)
        => (DomainUser)await base.UpdateAsync((InfrastructureUser)entity);

    public new async Task<DomainUser?> GetByIdAsync(long id)
        => (DomainUser)await base.GetByIdAsync(id);

    public new async Task<IReadOnlyList<DomainUser>> GetAllAsync()
        => (IReadOnlyList<DomainUser>)await base.GetAllAsync();

    public async Task<DomainUser?> GetByEmail(string email)
    {
        var user = await this.dbSet.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
        return (DomainUser?)user;
    }

    public async Task<DomainUser?> GetByDocument(string document)
    {
        var user = await this.dbSet.AsNoTracking().FirstOrDefaultAsync(u => u.Document == document);
        return (DomainUser?)user;
    }
}
