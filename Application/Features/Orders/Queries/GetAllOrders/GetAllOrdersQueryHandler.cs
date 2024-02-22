using System.Collections.Immutable;
using Application.Features.Orders.Contracts;
using Domain.Features.Orders.Repository;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Queries.GetAllOrders;

public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, Result<ImmutableList<OrderResponse>>>
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<GetAllOrdersQueryHandler> _logger;

    public GetAllOrdersQueryHandler(
        IOrderRepository repository,
        ILogger<GetAllOrdersQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<ImmutableList<OrderResponse>>> Handle(
        GetAllOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetAllAsync();
        return Result.Ok(result.Select(x => (OrderResponse)x).ToImmutableList());
    }
}
