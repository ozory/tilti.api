using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstractions;
using NetTopologySuite.Geometries;

namespace Domain.Shared.ValueObjects;

public class Location
{
    public Double Latitude { get; set; }
    public Double Longitude { get; set; }

    public Location(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
}
