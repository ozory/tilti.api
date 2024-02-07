using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Subscriptions.Commands.UpdateSubscription;

public class UpdateSubscriptionCommandHandler : ICommandHandler<UpdateSubscriptionCommand>
{
    public Task<Result> Handle(UpdateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
