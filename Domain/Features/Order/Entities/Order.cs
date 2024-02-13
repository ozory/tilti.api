
using System.Collections.ObjectModel;
using Domain.Abstractions;
using Domain.Enums;
using Domain.Features.Users.Entities;
using Domain.ValueObjects;
using FluentResults;

namespace Domain.Features.Order.Entities;

public class Order : Entity<Order>
{
    #region PROPERTIES

    internal List<Item> _items = new();
    internal List<Address> _addresses = new();

    public User User { get; protected set; } = null!;
    public User? Driver { get; protected set; }
    public Amount Amount { get; protected set; } = null!;

    public DateTime? RequestedTime { get; protected set; }
    public DateTime? AcceptanceTime { get; protected set; }
    public DateTime? CompletionTime { get; protected set; }
    public DateTime? CancelationTime { get; protected set; }

    public IReadOnlyCollection<Item> Items => new ReadOnlyCollection<Item>(_items);
    public IReadOnlyCollection<Address> Addresses => new ReadOnlyCollection<Address>(_addresses);

    public OrderStatus Status { get; protected set; }

    #endregion

    #region CONSTRUCTORS

    public static Order Create(User user, DateTime requestedTime, List<Address> address)
    {
        var order = new Order()
        {
            User = user,
            RequestedTime = requestedTime
        };

        order._addresses.AddRange(address);
        order.CreatedAt = DateTime.Now;
        order.Status = OrderStatus.PendingPayment;
        return order;
    }

    #endregion
}
