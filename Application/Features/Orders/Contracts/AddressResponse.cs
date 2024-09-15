using Domain.ValueObjects;

namespace Application.Features.Orders.Contracts;
public record AddressResponse(
    ushort AddressType,
    string Street,
    string Number,
    string Complment,
    string Neighborhood,
    string City,
    string ZipCode,
    Double Latitude,
    Double Longitude
)
{
    public static explicit operator AddressResponse(Address address)
       => new AddressResponse(
                (ushort)address.AddressType,
                address.Street,
                address.Number,
                address.Complment,
                address.Neighborhood,
                address.City,
                address.ZipCode,
                address.Latitude,
                address.Longitude
                );

    public static explicit operator Address(AddressResponse address)
       => new Address()
       {
           AddressType = (Domain.Orders.Enums.AddressType)address.AddressType,
           Street = address.Street,
           Number = address.Number,
           Complment = address.Complment,
           Neighborhood = address.Neighborhood,
           City = address.City,
           ZipCode = address.ZipCode,
           Latitude = address.Latitude,
           Longitude = address.Longitude
       };
}
