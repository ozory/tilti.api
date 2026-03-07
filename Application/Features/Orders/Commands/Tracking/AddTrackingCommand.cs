using Application.Features.Orders.Contracts;
using Application.Shared.Abstractions;
using Domain.ValueObjects;
using FluentResults;
namespace Application.Features.Orders.Commands.OrderTracking;

public sealed record AddTrackingCommand
(
    long OrderId,
    Double Latitude,
    Double Longitude

) : ICommand<Result<bool>>;


