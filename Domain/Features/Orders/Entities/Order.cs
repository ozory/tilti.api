
using System.Collections.ObjectModel;
using Domain.Abstractions;
using Domain.Enums;
using Domain.Features.Users.Entities;
using Domain.Shared.ValueObjects;
using Domain.ValueObjects;
using GeoPoint = NetTopologySuite.Geometries.Point;

namespace Domain.Features.Orders.Entities;

/// <summary>
/// Orders
/// </summary>
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

    public Location? Location { get; protected set; }

    public OrderStatus Status { get; protected set; }

    #endregion

    #region CONSTRUCTORS

    public Order() { }

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

        var startAddress = address.First(x => x.AddressType == AddressType.StartPoint);
        var startPointLatitude = startAddress.Latitude;
        var startPointLongitude = startAddress.Longitude;

        order.Location = new Location(startPointLatitude, startPointLongitude);

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="latitude"></param>
    /// <param name="longitude"></param>
    public void SetLocation(double latitude, double longitude)
    {
        this.Location = new Location(latitude, longitude);
    }

    #endregion
}
