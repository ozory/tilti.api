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
    private readonly IRateRepository _rateRepository;
    private readonly IOrderMessageRepository _messageRepository;
    private readonly TILTContext _context;
    private readonly IMediator _mediator;

    public UnitOfWork(
        TILTContext context,
        IMediator mediator,
        IUserRepository userRepository,
        IOrderRepository ordersRepository,
        ISubscriptionRepository subscriptionRepository,
        IPlanRepository planRepository,
        IRateRepository rateRepository,
        IOrderMessageRepository messageRepository)
    {
        this._context = context;
        this._mediator = mediator;
        this._userRepository = userRepository;
        this._ordersRepository = ordersRepository;
        this._subscriptionRepository = subscriptionRepository;
        this._planRepository = planRepository;
        this._rateRepository = rateRepository;
        this._messageRepository = messageRepository;
    }

    public IUserRepository UserRepository { get { return _userRepository; } }
    public IOrderRepository OrderRepository { get { return _ordersRepository; } }
    public ISubscriptionRepository SubscriptionRepository { get { return _subscriptionRepository; } }
    public IPlanRepository PlanRepository { get { return _planRepository; } }
    public IRateRepository RateRepository { get { return _rateRepository; } }
    public IOrderMessageRepository OrderMessageRepository { get { return _messageRepository; } }

    public async Task<bool> CommitAsync(CancellationToken cancellationToken)
    {
        bool returnValue = true;
        using (var dbContextTransaction = _context.Database.BeginTransaction())
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                dbContextTransaction.Commit();

                ExecuteDomainEvents();
                await Task.CompletedTask;
            }
            catch (Exception)
            {
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

        domainEntities
            .ToList()
            .ForEach(entity => entity.Entity.ClearEvents());

        _ = Task.Run(() =>
        {
            Parallel.ForEach(domainEvents, domainEvent => { _mediator.Publish(domainEvent); });
        });
    }
}
