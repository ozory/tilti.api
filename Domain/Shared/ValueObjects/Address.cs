using Domain.Enums;

namespace Domain.ValueObjects;

public class Address
{
    public AddressType AddressType { get; init; } = AddressType.StartPoint;

    public string Street { get; init; } = null!;
    public string Number { get; init; } = null!;
    public string Complment { get; init; } = null!;
    public string Neighborhood { get; init; } = null!;
    public string City { get; init; } = null!;
    public string ZipCode { get; init; } = null!;

    public Double Latitude { get; init; }
    public Double Longitude { get; init; }

}
