using Application.Shared.Abstractions;
using Domain.Abstractions;
using Domain.Features.Orders.Repository;
using Domain.Features.Plans.Repository;
using Domain.Features.Subscriptions.Repository;
using Domain.Features.Users.Repository;
using Domain.Shared.Abstractions;
using MediatR;

namespace Infrastructure.Data.Postgreesql.Shared.Abstractions;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly IUserRepository _userRepository;
    private readonly IOrderRepository _ordersRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IPlanRepository _planRepository;
    private readonly TILTContext _context;
    private readonly IMediator _mediator;

    public UnitOfWork(
        TILTContext context,
        IMediator mediator,
        IUserRepository userRepository,
        IOrderRepository ordersRepository,
        ISubscriptionRepository subscriptionRepository,
        IPlanRepository planRepository)
    {
        this._context = context;
        this._mediator = mediator;
        this._userRepository = userRepository;
        this._ordersRepository = ordersRepository;
        this._subscriptionRepository = subscriptionRepository;
        this._planRepository = planRepository;
    }

    public IUserRepository UserRepository { get { return _userRepository; } }
    public IOrderRepository OrderRepository { get { return _ordersRepository; } }
    public ISubscriptionRepository SubscriptionRepository { get { return _subscriptionRepository; } }
    public IPlanRepository PlanRepository { get { return _planRepository; } }

    public async Task<bool> CommitAsync(CancellationToken cancellationToken)
    {
        bool returnValue = true;
        using (var dbContextTransaction = _context.Database.BeginTransaction())
        {
            try
            {
                //  await _context.SaveChangesAsync(cancellationToken);
                // dbContextTransaction.Commit();

                ExecuteDomainEvents();
            }
            catch (Exception)
            {
                //Log Exception Handling message                      
                returnValue = false;
                dbContextTransaction.Rollback();
                throw;
            }
        }

        return returnValue;

    }

    private void ExecuteDomainEvents()
    {
        var domainEntities = _context.ChangeTracker
            .Entries<Entity>()
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
