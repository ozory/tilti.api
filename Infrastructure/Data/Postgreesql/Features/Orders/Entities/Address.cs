using DomainOrder = Domain.Features.Orders.Entities.Order;
using DomainAddress = Domain.ValueObjects.Address;
using Domain.Enums;

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

    public static explicit operator Address(DomainAddress address)
    {
        return new Address()
        {
            AddressType = (ushort)address.AddressType,
            City = address.City,
            Street = address.Street,
            Number = address.Number,
            Complment = address.Complment,
            ZipCode = address.ZipCode,
            Neighborhood = address.Neighborhood,
            Latitude = address.Latitude,
            Longitude = address.Longitude,
        };
    }

    public static explicit operator DomainAddress(Address address)
    {
        return new DomainAddress()
        {
            AddressType = (AddressType)address.AddressType,
            City = address.City,
            Street = address.Street,
            Number = address.Number,
            Complment = address.Complment,
            ZipCode = address.ZipCode,
            Neighborhood = address.Neighborhood,
            Latitude = address.Latitude,
            Longitude = address.Longitude,
        };
    }
}
