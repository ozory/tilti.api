using System.Collections.Immutable;
using Application.Features.Orders.Contracts;
using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Orders.Queries.GetOrderTracking;

public class GetOrderTrackingQuery : IQuery<Result<ImmutableList<OrderTrackingResponse>>>
{
    public long OrderId { get; set; }
    public long TrackingId { get; set; }
}
