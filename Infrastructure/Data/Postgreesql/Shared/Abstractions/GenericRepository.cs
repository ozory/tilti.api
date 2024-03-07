using Domain.Abstractions;
using Domain.Shared.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

using DomainUser = Domain.Features.Users.Entities.User;
using InfrastructureUser = Infrastructure.Data.Postgreesql.Features.Users.Entities.User;

namespace Infrastructure.Data.Postgreesql.Shared;

public abstract class GenericRepository<TSource, TDestination>
    where TSource : Entity
    where TDestination : AbstractEntity
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

    public async Task<IReadOnlyList<TSource>> GetAllAsync()
    {
        var resources = await this.dbSet.AsNoTracking().ToListAsync();
        var userList = resources.Select(u => u as TSource).ToList();
        return userList!;
    }

    public async Task<TSource?> GetByIdAsync(long id)
    {
        var user = await this.dbSet.FindAsync(id);
        return user as TSource;
    }

    public async Task<TSource> SaveAsync(TSource entity)
    {
        await this.dbSet.AddAsync((entity as TDestination)!);
        return entity;
    }

    public async Task SaveChangesAsync()
    {
        _context.SaveChanges();

        var domainEntities = _context.ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ToList()
            .ForEach(entity => entity.Entity.ClearEvents());

        // Dispatch domain events after save changes
        foreach (var domainEvent in domainEvents)
            await _mediator.Publish(domainEvent);
    }

    public async Task<TSource> UpdateAsync(TSource entity)
    {
        var destination = entity as TDestination;
        dbSet.Attach(destination!);
        _context.Entry(destination!).State = EntityState.Modified;
        await Task.CompletedTask;
        return (destination as TSource)!;
    }
}


