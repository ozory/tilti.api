using System.Collections.Immutable;
using Application.Features.Orders.Contracts;
using FluentResults;
using MediatR;

namespace Application.Features.Orders.Queries.GetAllOrders;

public class GetAllOrdersQuery : IRequest<Result<ImmutableList<OrderResponse>>>
{

}
