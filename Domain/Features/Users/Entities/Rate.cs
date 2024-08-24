using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstractions;
using Domain.Features.Orders.Entities;

namespace Domain.Features.Users.Entities;

public class Rate : Entity
{
    #region PROPERTIES

    public long SourceUserId { get; protected set; }
    public long TargetUserId { get; protected set; }
    public long OrderId { get; protected set; }
    public float Value { get; protected set; }

    public User? SourceUser { get; protected set; } = null!;
    public User? TargetUser { get; protected set; } = null!;
    public Order? Order { get; protected set; } = null!;

    #endregion

    #region CONSTRUCTORS

    public Rate() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceUserId"></param>
    /// <param name="targetUserId"></param>
    /// <param name="value"></param>
    private Rate(Order order, User sourceUser, User targetUser, float value)
    {
        SourceUser = sourceUser;
        TargetUser = targetUser;
        Value = value;
    }

    public static Rate Create(Order order, User sourceUser, User targetUser, float value)
    {
        var rate = new Rate(order, sourceUser, targetUser, value);
        return rate;
    }

    public static Rate Create(long OrderId, long sourceUserId, long targetUserId, float value)
    {
        var rate = new Rate(Order.Create(OrderId), User.Create(sourceUserId), User.Create(targetUserId), value);
        return rate;
    }

    #endregion
}
