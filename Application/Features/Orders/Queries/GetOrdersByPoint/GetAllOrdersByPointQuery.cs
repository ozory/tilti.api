using System.Collections.Immutable;
using Application.Features.Orders.Contracts;
using Domain.Shared.Enums;
using FluentResults;
using MediatR;

namespace Application.Features.Orders.Queries.GetOrdersByPoint;

public class GetAllOrdersByPointQuery : IRequest<Result<ImmutableList<OrderResponse>>>
{
    public long? DriverId { get; set; }
    public OrderType OrderType { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public double? DestinationLatitude { get; set; }
    public double? DestinationLongitude { get; set; }
}
