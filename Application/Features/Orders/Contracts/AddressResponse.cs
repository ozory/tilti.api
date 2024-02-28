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
    public static implicit operator AddressResponse(Address address)
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
}
