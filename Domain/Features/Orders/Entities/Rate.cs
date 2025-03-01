using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstractions;
using Domain.Features.Orders.Entities;
using Domain.Features.Users.Entities;

namespace Domain.Features.Orders.Entities;

public class Rate : Entity
{
    #region PROPERTIES

    public long SourceUserId { get; protected set; }
    public long TargetUserId { get; protected set; }
    public long OrderId { get; protected set; }
    public float Value { get; protected set; }
    public string? Description { get; protected set; } = string.Empty;
    public string? Tags { get; protected set; } = string.Empty;

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
    private Rate(
        Order order,
        User sourceUser,
        User targetUser,
        float value,
        string? description = null,
        string? tags = null)
    {
        Order = order;
        SourceUser = sourceUser;
        TargetUser = targetUser;
        Value = value;
        Description = description;
        Tags = tags;
    }

    public static Rate Create(
        Order order,
        User sourceUser,
        User targetUser,
        float value,
        string? description = null,
        string? tags = null)
    {
        var rate = new Rate(order, sourceUser, targetUser, value, description, tags);
        return rate;
    }

    #endregion
}
