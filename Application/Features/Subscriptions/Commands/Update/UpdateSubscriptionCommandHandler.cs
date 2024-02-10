using Application.Shared.Abstractions;
using Domain.Features.Plans.Repository;
using Domain.Features.Subscription.Repository;
using Domain.Features.Users.Repository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Features.Subscriptions.Commands.Update;

public class UpdateSubscriptionCommandHandler : ICommandHandler<UpdateSubscriptionCommand>
{
    private readonly ISubscriptionRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly IPlanRepository _planRepository;
    private readonly ILogger<UpdateSubscriptionCommandHandler> _logger;

    public UpdateSubscriptionCommandHandler(
        ILogger<UpdateSubscriptionCommandHandler> logger,
        ISubscriptionRepository repository,
        IUserRepository userRepository,
        IPlanRepository planRepository)
    {
        _repository = repository;
        _logger = logger;
        _userRepository = userRepository;
        _planRepository = planRepository;
    }

    public Task<Result> Handle(UpdateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
