using System.Collections.Immutable;
using Application.Features.Orders.Contracts;
using Domain.Features.Orders.Entities;
using Domain.Features.Orders.Repository;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;

namespace Application.Features.Orders.Queries.GetOrdersByPoint;

public class GetAllOrdersByPointQueryHandler : IRequestHandler<GetAllOrdersByPointQuery, Result<ImmutableList<OrderResponse>>>
{
    private readonly IOrderRepository _repository;
    private readonly IRejectRepository _rejectionRepository;
    private readonly ILogger<GetAllOrdersByPointQueryHandler> _logger;

    public GetAllOrdersByPointQueryHandler(
        IOrderRepository repository,
        ILogger<GetAllOrdersByPointQueryHandler> logger,
        IRejectRepository rejectionRepository)
    {
        _repository = repository;
        _logger = logger;
        _rejectionRepository = rejectionRepository;
    }

    public async Task<Result<ImmutableList<OrderResponse>>> Handle(
        GetAllOrdersByPointQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Rejection?> rejections = [];

        if (request.DriverId.HasValue) rejections = await _rejectionRepository.GetRejectionsByUser(request.DriverId!.Value);

        var driveDestionation = request.DestinationLatitude.HasValue && request.DestinationLongitude.HasValue ?
        new Point(request.DestinationLongitude!.Value, request.DestinationLatitude!.Value) : null;

        var result = await _repository.GetOrdersByPoint(
            new Point(request.Longitude, request.Latitude),
            driveDestionation,
            request.OrderType);

        // retornar todas as ordens que não estão em rejeição
        var availables = result.Where(x => !rejections.Any(r => r!.OrderId == x!.Id));

        return Result.Ok(availables.Select(x => (OrderResponse)x!).ToImmutableList());
    }
}
