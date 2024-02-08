
using System.Collections.ObjectModel;
using Domain.Abstractions;
using Domain.Enums;
using Domain.Features.Users.Entities;
using Domain.ValueObjects;
using FluentResults;

namespace Domain.Features.Order.Entities;

public class Order : Entity<Order>
{
    internal List<Item> _items = new();
    internal List<Address> _addresses = new();

    public User Client { get; protected set; } = null!;
    public User? Attendant { get; protected set; }

    public Amount Amount { get; protected set; } = null!;

    public TimeOnly? RequestedTime { get; protected set; }
    public TimeOnly? AcceptanceTime { get; protected set; }
    public TimeOnly? CompletionTime { get; protected set; }
    public TimeOnly? CancelationTime { get; protected set; }

    public IReadOnlyCollection<Item> Items => new ReadOnlyCollection<Item>(_items);
    public IReadOnlyCollection<Address> Addresses => new ReadOnlyCollection<Address>(_addresses);

    public OrderStatus Status { get; protected set; }
}
