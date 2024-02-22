using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Orders.Contracts;
using Application.Features.Users.Contracts;
using FluentResults;
using MediatR;

namespace Application.Features.Orders.Queries.GetAllOrders;

public class GetAllOrdersQuery : IRequest<Result<ImmutableList<OrderResponse>>>
{

}
