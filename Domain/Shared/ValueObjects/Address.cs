using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.ValueObjects;

public class Address
{
    public AddressType AddressType { get; protected set; } = AddressType.StartPoint;

    public string Street { get; protected set; } = null!;
    public string Number { get; protected set; } = null!;
    public string Complment { get; protected set; } = null!;
    public string Neighborhood { get; protected set; } = null!;
    public string City { get; protected set; } = null!;
    public string ZipCode { get; protected set; } = null!;

    public Double Latitude { get; protected set; }
    public Double Longitude { get; protected set; }
}
