
using System.Collections.ObjectModel;
using Domain.Abstractions;
using Domain.Enums;
using Domain.Features.Users.Entities;
using Domain.ValueObjects;

namespace Domain.Features.Orders.Entities;

public class Order : Entity
{
    #region PROPERTIES

    internal List<Item> _items = new();
    internal List<Address> _addresses = new();

    public long UserId { get; protected set; }
    public long? DriverId { get; protected set; }

    public User? User { get; protected set; } = null!;
    public User? Driver { get; protected set; }
    public Amount Amount { get; protected set; } = null!;

    public int DistanceInKM { get; protected set; }
    public int DurationInSeconds { get; protected set; }

    public DateTime? RequestedTime { get; protected set; }
    public DateTime? AcceptanceTime { get; protected set; }
    public DateTime? CompletionTime { get; protected set; }
    public DateTime? CancelationTime { get; protected set; }

    public IReadOnlyCollection<Item> Items => new ReadOnlyCollection<Item>(_items);
    public IReadOnlyCollection<Address> Addresses => new ReadOnlyCollection<Address>(_addresses);

    public OrderStatus Status { get; protected set; }

    #endregion

    #region CONSTRUCTORS

    private Order() { }

    public static Order Create(
        long? id,
        User? user,
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
        order.RequestedTime = DateTime.Now;
        order.Status = OrderStatus.PendingPayment;
        return order;
    }

    #endregion

    #region METHODS

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    public void SetUser(User? user)
    {
        this.UserId = user?.Id ?? 0;
        this.User = user;
    }

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
        if (driver is null) return;
        this.DriverId = driver.Id;
        this.Driver = driver;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="driver"></param>
    public void RemoveDriver()
    {
        this.DriverId = null;
        this.Driver = null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="date"></param>
    public void SetUpdatedAt(DateTime date) => this.UpdatedAt = date;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="amount"></param>
    public void SetAmount(Decimal value) => this.Amount = new Amount(value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void SetDistanceInKM(int value) => this.DistanceInKM = value;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void SetDurationInSeconds(int value) => this.DurationInSeconds = value;

    #endregion
}
