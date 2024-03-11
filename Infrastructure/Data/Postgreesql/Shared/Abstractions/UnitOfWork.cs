using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Domain.Abstractions;
using Domain.Features.Users.Entities;
using Domain.Features.Users.Repository;
using Domain.Shared.Abstractions;
using MediatR;

namespace Infrastructure.Data.Postgreesql.Shared.Abstractions;

public class UnitOfWork : IUnitOfWork
{
    private readonly IUserRepository userRepository;
    private readonly TILTContext _context;

    private readonly IMediator _mediator;

    public UnitOfWork(
        IUserRepository userRepository,
        TILTContext context,
        IMediator mediator)
    {
        this.userRepository = userRepository;
        this._context = context;
        _mediator = mediator;
    }

    public IGenericRepository<TEntity> GetRepository<TEntity>()
        where TEntity : Entity
    {
        var repository = typeof(TEntity).Name switch
        {
            nameof(User) => userRepository,
            _ => throw new NotImplementedException(),
        };

        return (IGenericRepository<TEntity>)repository;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);

        ExecuteDomainEvents();
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
}
