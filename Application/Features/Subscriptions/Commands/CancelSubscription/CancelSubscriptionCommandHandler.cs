using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Subscriptions.Commands.CancelSubscription;

public class CancelSubscriptionCommandHandler : ICommandHandler<CancelSubscriptionCommand>
{
    public Task<Result> Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
