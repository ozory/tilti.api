using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstractions;

namespace Domain.Shared.ValueObjects;

public class Position : ValueObject<Position>
{
    public Double Latitude { get; init; }
    public Double Longitude { get; init; }

}
