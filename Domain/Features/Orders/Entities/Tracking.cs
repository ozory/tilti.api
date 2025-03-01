using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstractions;
using Domain.Shared.ValueObjects;
using GeoPoint = NetTopologySuite.Geometries.Point;

namespace Domain.Features.Orders.Entities;

public class Tracking : Entity
{
    #region PROPERTIES

    public long OrderId { get; protected set; }
    public GeoPoint Point { get; protected set; } = null!;
    public Location Location { get; set; } = null!;
    public Order Order { get; protected set; } = null!;

    #endregion

    #region CONSTRUCTORS

    public Tracking() { }

    private Tracking(long orderId, Location location)
    {
        Order = Order.Create(orderId);
        OrderId = orderId;
        Location = location;
        this.Point = new GeoPoint(location.Latitude, location.Longitude);
    }

    public static Tracking Create(long orderId, Location location)
    {
        return new Tracking(orderId, location);
    }

    #endregion
}
