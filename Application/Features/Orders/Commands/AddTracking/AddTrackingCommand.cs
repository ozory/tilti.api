using Application.Features.Orders.Contracts;
using Application.Shared.Abstractions;

namespace Application.Features.Orders.Commands.AddTracking;

public sealed record AddTrackingCommand(
    long OrderId,
    long DriverId,
    double Latitude,
    double Longitude
) : ICommand<OrderResponse>;