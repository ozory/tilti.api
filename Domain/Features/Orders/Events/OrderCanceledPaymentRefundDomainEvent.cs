using Domain.Shared.Abstractions;

namespace Domain.Features.Orders.Events;

public record OrderCanceledPaymentRefundDomainEvent(
    long OrderId,
    long UserId,
    decimal Amount,
    List<string> Reason,
    string Description,
    DateTime CancellationTime
) : IDomainEvent;