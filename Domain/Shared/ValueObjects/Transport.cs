using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstractions;

namespace Domain.ValueObjects;

public class Transport : ValueObject<Transport>
{
    public Transport(Transport value) : base(value)
    {
    }

    public Transport() : base() { }

    public static Transport CreateTransport(
        string name,
        string description,
        ushort year,
        string plate,
        string model)
    {
        var transport = new Transport();
        transport.Name = new Name(name);
        transport.Description = new Description(description);
        transport.Year = year;
        transport.Plate = plate;
        transport.Model = model;
        return transport;
    }

    public Name Name { get; protected set; } = null!;
    public Description Description { get; protected set; } = null!;
    public ushort Year { get; protected set; }
    public string Plate { get; protected set; } = null!;
    public string Model { get; protected set; } = null!;
}
