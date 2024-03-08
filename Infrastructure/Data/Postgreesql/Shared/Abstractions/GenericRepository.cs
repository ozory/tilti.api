using Domain.Abstractions;
using Domain.Shared.Abstractions;
using Infrastructure.Data.Postgreesql.Shared.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

using DomainUser = Domain.Features.Users.Entities.User;
using InfrastructureUser = Infrastructure.Data.Postgreesql.Features.Users.Entities.User;

namespace Infrastructure.Data.Postgreesql.Shared;

public abstract class GenericRepository<TDestination>
    where TDestination : InfrastructureEntity
{
    protected readonly TILTContext _context;
    private readonly IMediator _mediator;

    internal DbSet<TDestination> dbSet;

    protected GenericRepository(TILTContext context, IMediator mediatr)
    {
        _context = context;
        _mediator = mediatr;
        this.dbSet = _context.Set<TDestination>();
    }

    public virtual async Task<IReadOnlyList<TDestination>> GetAllAsync()
    {
        var resources = await this.dbSet.AsNoTracking().ToListAsync();
        var userList = resources.Select(u => u).ToList();
        return userList!;
    }

    public virtual async Task<TDestination?> GetByIdAsync(long id)
    {
        var user = await this.dbSet.FindAsync(id);
        return user;
    }

    public virtual async Task<TDestination> SaveAsync(TDestination entity)
    {
        await this.dbSet.AddAsync(entity);
        return entity;
    }

    public virtual async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        // _context.SaveChanges();
        ExecuteDomainEvents();

        // Dispatch domain events after save changes
        await Task.CompletedTask;
    }

    private void ExecuteDomainEvents()
    {
        var domainEntities = _context.ChangeTracker
                    .Entries<InfrastructureEntity>()
                    .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ToList()
            .ForEach(entity => entity.Entity.ClearEvents());


        _ = Task.Run(() =>
        {
            Parallel.ForEach(domainEvents, domainEvent => { _mediator.Publish(domainEvent); });
        });
    }

    public virtual async Task<TDestination> UpdateAsync(TDestination entity)
    {
        dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        await Task.CompletedTask;
        return entity;
    }
}


