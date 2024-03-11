using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Application.Shared.Abstractions;
using Domain.Abstractions;
using Domain.Features.Orders.Entities;
using Domain.Features.Orders.Repository;
using Domain.Features.Plans.Entities;
using Domain.Features.Plans.Repository;
using Domain.Features.Subscriptions.Entities;
using Domain.Features.Subscriptions.Repository;
using Domain.Features.Users.Entities;
using Domain.Features.Users.Repository;
using Domain.Shared.Abstractions;
using Infrastructure.Data.Postgreesql.Features.Orders.Repository;
using Infrastructure.Data.Postgreesql.Features.Plans.Repository;
using Infrastructure.Data.Postgreesql.Features.Subscriptions.Repository;
using MediatR;

namespace Infrastructure.Data.Postgreesql.Shared.Abstractions;

public class UnitOfWork : IUnitOfWork
{
    private readonly IUserRepository _userRepository;
    private readonly IOrderRepository _ordersRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IPlanRepository _planRepository;
    private readonly ISecurityRepository _securityRepository;
    private readonly TILTContext _context;
    private readonly IMediator _mediator;

    public UnitOfWork(
        IUserRepository userRepository,
        TILTContext context,
        IMediator mediator,
        IOrderRepository ordersRepository,
        ISubscriptionRepository subscriptionRepository,
        IPlanRepository planRepository,
        ISecurityRepository securityRepository)
    {
        this._context = context;
        this._mediator = mediator;
        this._userRepository = userRepository;
        this._ordersRepository = ordersRepository;
        this._subscriptionRepository = subscriptionRepository;
        this._planRepository = planRepository;
        this._securityRepository = securityRepository;
    }

    public IUserRepository GetUserRepository() => _userRepository;
    public IOrderRepository GetOrderRepository() => _ordersRepository;
    public ISubscriptionRepository GetSubscriptionRepository() => _subscriptionRepository;
    public IPlanRepository GetPlanRepository() => _planRepository;
    public ISecurityRepository GetSecurityRepository() => _securityRepository;

    public async Task CommitAsync(CancellationToken cancellationToken)
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
