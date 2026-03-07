using Application.Shared.Abstractions;
using Domain.Features.Orders.Events;

namespace Application.Features.Orders.Events;

public class CreateMessageEventHandler : INotificationHandler<CreateMessageDomainEvent>
{
    public Task Handle(CreateMessageDomainEvent notification, CancellationToken cancellationToken)
    {
        // TODO: Send push notification
        throw new NotImplementedException();
    }
}
