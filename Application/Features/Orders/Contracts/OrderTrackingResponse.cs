using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Orders.Entities;

namespace Application.Features.Orders.Contracts;

public record OrderTrackingResponse(
    long TrackingId,
    long OrderId,
    DateTime CreatedAt,
    double Latitude,
    double Longitude
)
{
    public static explicit operator OrderTrackingResponse(Tracking tracking)
    {
        return new OrderTrackingResponse(
            tracking.Id,
            tracking.OrderId,
            tracking.CreatedAt,
            tracking.Location.Latitude,
            tracking.Location.Longitude
        );
    }
}
