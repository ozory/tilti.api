using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstractions;
using Domain.Features.Users.Entities;

namespace Domain.Features.Orders.Entities;

public class Rejection : Entity
{
    #region PROPERTIES

    public long OrderId { get; protected set; }
    public long DriverId { get; protected set; }
    public User User { get; protected set; } = null!;
    public Order Order { get; protected set; } = null!;

    #endregion

    #region CONSTRUCTORS

    public static Rejection Create(long? rejectionId)
    {
        return new Rejection() { Id = rejectionId ?? 0 };
    }

    #endregion

    #region METHODS

    /// <summary>
    /// 
    /// </summary>
    /// <param name="driver"></param>
    public void SetDriver(User driver)
    {
        this.User = driver;
        this.DriverId = driver.Id;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="order"></param>
    public void SetOrder(Order order)
    {
        this.Order = order;
        this.OrderId = order.Id;
    }

    #endregion
}
