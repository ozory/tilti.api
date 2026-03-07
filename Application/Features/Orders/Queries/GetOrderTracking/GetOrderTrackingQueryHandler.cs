using System.Collections.Immutable;
using Application.Features.Orders.Contracts;
using Application.Shared.Abstractions;
using Domain.Features.Orders.Repository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Queries.GetOrderTracking;

public class GetOrderTrackingQueryHandler : IQueryHandler<GetOrderTrackingQuery, ImmutableList<OrderTrackingResponse>>
{
    private readonly ITrackingRepository _repository;
    private readonly ILogger<GetOrderTrackingQueryHandler> _logger;

    public GetOrderTrackingQueryHandler(
        ITrackingRepository repository,
        ILogger<GetOrderTrackingQueryHandler> logger,
        IRejectRepository rejectionRepository)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<ImmutableList<OrderTrackingResponse>>> Handle(
        GetOrderTrackingQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetTrackingByOrder(request.OrderId, request.TrackingId, cancellationToken);
        return Result.Ok(result.Select(x => (OrderTrackingResponse)x).ToImmutableList());
    }
}
