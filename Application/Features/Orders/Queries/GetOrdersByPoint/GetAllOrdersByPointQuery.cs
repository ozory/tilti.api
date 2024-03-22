using System.Collections.Immutable;
using Application.Features.Orders.Contracts;
using FluentResults;
using MediatR;

namespace Application.Features.Orders.Queries.GetOrdersByPoint;

public class GetAllOrdersByPointQuery : IRequest<Result<ImmutableList<OrderResponse>>>
{
    public long? DriverId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
