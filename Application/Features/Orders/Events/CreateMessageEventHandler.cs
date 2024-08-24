using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Orders.Events;
using MediatR;

namespace Application.Features.Orders.Events;

public class CreateMessageEventHandler : INotificationHandler<CreateMessageDomainEvent>
{
    public Task Handle(CreateMessageDomainEvent notification, CancellationToken cancellationToken)
    {
        // TODO: Send push notification
        throw new NotImplementedException();
    }
}
