using System.Collections.Immutable;
using Application.Features.Orders.Contracts;
using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Orders.Queries.GetOrdersByUser;

public record GetOrdersByUserQuery(long UserId) : IQuery<Result<ImmutableList<OrderResponse>>>;