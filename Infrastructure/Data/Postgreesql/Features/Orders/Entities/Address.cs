namespace Infrastructure.Data.Postgreesql.Features.Orders.Entities;

public class Address
{
    public ushort AddressType { get; set; }

    public string Street { get; set; } = null!;
    public string Number { get; set; } = null!;
    public string Complment { get; set; } = null!;
    public string Neighborhood { get; set; } = null!;
    public string City { get; set; } = null!;
    public string ZipCode { get; set; } = null!;

    public Double Latitude { get; set; }
    public Double Longitude { get; set; }
}
