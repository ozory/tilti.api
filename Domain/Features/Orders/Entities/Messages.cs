using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstractions;
using Domain.Features.Users.Entities;

namespace Domain.Features.Orders.Entities;

public class Message : Entity
{
    #region PROPERTIES

    public long SourceUserId { get; protected set; }
    public long TargetUserId { get; protected set; }
    public long OrderId { get; protected set; }
    public string Value { get; protected set; } = string.Empty;
    public Order Order { get; protected set; } = null!;
    public User? SourceUser { get; protected set; } = null!;
    public User? TargetUser { get; protected set; } = null!;

    #endregion

    #region CONSTRUCTORS

    public Message() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceUserId"></param>
    /// <param name="targetUserId"></param>
    /// <param name="orderId"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Message Create(long sourceUserId, long targetUserId, long orderId, string value)
    {
        return new Message()
        {
            SourceUserId = sourceUserId,
            TargetUserId = targetUserId,
            OrderId = orderId,
            Value = value,
            Order = Order.Create(orderId),
            SourceUser = User.Create(sourceUserId),
            TargetUser = User.Create(targetUserId)
        };
    }

    #endregion
}
