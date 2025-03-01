using System.Collections.Immutable;
using Application.Features.Orders.Contracts;
using FluentResults;
using MediatR;

namespace Application.Features.Orders.Queries.GetOrdersByPoint;

public class GetOrderTrackingQuery : IRequest<Result<ImmutableList<OrderTrackingResponse>>>
{
    public long OrderId { get; set; }
    public long TrackingId { get; set; }
}
