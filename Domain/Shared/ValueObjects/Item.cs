using Domain.Features.Users.Entities;

namespace Domain.ValueObjects;

public class Item
{
    public User? User { get; protected set; }
    public uint Quantity { get; protected set; }
    public Name Name { get; protected set; } = null!;
    public Amount Amount { get; protected set; } = null!;
    public Description Description { get; protected set; } = null!;
}
