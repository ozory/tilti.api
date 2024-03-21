using System.Collections.Immutable;
using Application.Features.Orders.Contracts;
using Application.Shared.Abstractions;
using Domain.ValueObjects;
using FluentResults;
namespace Application.Features.Orders.Commands.CloseOrder;

public sealed record CloseExpireOrdersCommand
(
    DateTime requestedTime
) : ICommand<Result<ImmutableList<OrderResponse>>>;


