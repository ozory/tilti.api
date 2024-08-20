using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstractions;

namespace Domain.Features.Users.Entities;

public class Rate : Entity
{
    #region PROPERTIES

    public long SourceUserId { get; protected set; }
    public long TargetUserId { get; protected set; }
    public float Value { get; protected set; }

    public User? SourceUser { get; protected set; } = null!;
    public User? TargetUser { get; protected set; } = null!;

    #endregion

    #region CONSTRUCTORS

    public Rate() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceUserId"></param>
    /// <param name="targetUserId"></param>
    /// <param name="value"></param>
    private Rate(User sourceUser, User targetUser, float value)
    {
        SourceUser = sourceUser;
        TargetUser = targetUser;
        Value = value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceUserId"></param>
    /// <param name="targetUserId"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Rate Create(User sourceUser, User targetUser, float value)
    {
        var rate = new Rate(sourceUser, targetUser, value);
        return rate;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceUserId"></param>
    /// <param name="targetUserId"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Rate Create(long sourceUserId, long targetUserId, float value)
    {
        var rate = new Rate(User.Create(sourceUserId), User.Create(targetUserId), value);
        return rate;
    }

    #endregion
}
