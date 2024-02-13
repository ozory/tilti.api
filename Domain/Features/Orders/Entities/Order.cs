
using System.Collections.ObjectModel;
using Domain.Abstractions;
using Domain.Enums;
using Domain.Features.Users.Entities;
using Domain.ValueObjects;
using FluentResults;
using Microsoft.VisualBasic;

namespace Domain.Features.Orders.Entities;

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

    public static Order Create(
        long? id,
        User user,
        DateTime requestedTime,
        List<Address> address,
        DateTime? createdAt)
    {
        var order = new Order()
        {
            Id = id ?? 0,
            User = user,
            RequestedTime = requestedTime
        };

        order._addresses.AddRange(address);
        order.CreatedAt = createdAt ?? DateTime.Now;
        order.Status = OrderStatus.PendingPayment;
        return order;
    }

    #endregion

    #region METHODS

    /// <summary>
    /// Altera o status da assinatura
    /// </summary>
    /// <param name="status"></param> 
    public void SetStatus(OrderStatus status)
    {
        if (status != this.Status)
        {
            this.Status = status;
        }
        return;
    }

    public void SetAcceptanceTime(DateTime? dateAndTime) => this.AcceptanceTime = dateAndTime;
    public void SetCompletionTime(DateTime? dateAndTime) => this.CompletionTime = dateAndTime;
    public void SetCancelationTime(DateTime? dateAndTime)
    {
        this.CancelationTime = dateAndTime;
        SetAcceptanceTime(null);
        SetCompletionTime(null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="driver"></param>
    public void SetDriver(User? driver)
    {
        this.Driver = driver;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="driver"></param>
    public void RemoveDriver(User driver)
    {
        this.Driver = null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="date"></param>
    public void SetUpdatedAt(DateTime date) => this.UpdatedAt = date;

    #endregion
}