using Domain.Shared.Messaging;

namespace Application.Shared.Abstractions;

public interface INotificationHandler<in TNotification>
    where TNotification : INotification
{
    Task Handle(TNotification notification, CancellationToken cancellationToken);
}