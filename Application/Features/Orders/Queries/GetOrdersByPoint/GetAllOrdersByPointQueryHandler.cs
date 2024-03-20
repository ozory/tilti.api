using System.Collections.Immutable;
using Application.Features.Orders.Contracts;
using Domain.Features.Orders.Repository;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;

namespace Application.Features.Orders.Queries.GetOrdersByPoint;

public class GetAllOrdersByPointQueryHandler : IRequestHandler<GetAllOrdersByPointQuery, Result<ImmutableList<OrderResponse>>>
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<GetAllOrdersByPointQueryHandler> _logger;

    public GetAllOrdersByPointQueryHandler(
        IOrderRepository repository,
        ILogger<GetAllOrdersByPointQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<ImmutableList<OrderResponse>>> Handle(
        GetAllOrdersByPointQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetOrdersByPoint(new Point(request.Longitude, request.Latitude));
        return Result.Ok(result.Select(x => (OrderResponse)x!).ToImmutableList());
    }
}
