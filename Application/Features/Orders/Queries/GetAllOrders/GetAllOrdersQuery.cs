using System.Collections.Immutable;
using Application.Features.Orders.Contracts;
using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Orders.Queries.GetAllOrders;

public class GetAllOrdersQuery : IQuery<Result<ImmutableList<OrderResponse>>>
{
}
