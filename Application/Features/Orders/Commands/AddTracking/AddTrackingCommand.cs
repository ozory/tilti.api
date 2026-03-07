using Application.Features.Orders.Contracts;
using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Orders.Commands.AddTracking;

public sealed record AddTrackingCommand(
    long OrderId,
    long DriverId,
    double Latitude,
    double Longitude
) : ICommand<Result<OrderResponse>>;