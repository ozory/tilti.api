using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Domain.Abstractions;
using Domain.Features.Payments.Enums;
using Domain.Features.Users.Entities;
using Domain.ValueObjects;

namespace Domain.Features.Payments.Entities;

public class Payment : Entity
{
    #region PROPERTIES

    public Guid? Identifier { get; protected set; } = null!;

    public long UserId { get; protected set; }
    public User User { get; protected set; } = null!;
    public long ReceiverId { get; protected set; }
    public User Receiver { get; protected set; } = null!;

    public Amount Amount { get; protected set; } = null!;

    public PaymentStatus Status { get; protected set; } = PaymentStatus.Pending;
    public PaymentType Type { get; protected set; } = PaymentType.CreditCard;
    public DateTime? ApprovedAt { get; protected set; }
    public DateTime? CancelledAt { get; protected set; }

    #endregion

    #region CONSTRUCTORS

    protected Payment()
    {
    }

    public static Payment Create(
        long? id,
        Guid? identifier,
        User? user,
        PaymentType type,
        DateTime requestedTime)
    {
        var payment = new Payment
        {
            Id = id ?? 0,
            User = user ?? throw new ArgumentNullException(nameof(user)),
            UserId = user.Id,
            ReceiverId = user.Id,
            Receiver = user,
            Type = type,
            Identifier = identifier,
            CreatedAt = requestedTime
        };

        return payment;
    }

    #endregion

    #region METHODS

    /// <summary>
    /// Approve the payment
    /// </summary>
    public void ApprovePayment()
    {
        Status = PaymentStatus.Approved;
        ApprovedAt = DateTime.Now;
    }

    /// <summary>
    /// Cancel the payment
    /// </summary>
    public void CancelPayment()
    {
        Status = PaymentStatus.Cancelled;
        CancelledAt = DateTime.Now;
    }

    /// <summary>
    /// Set the amount of the payment
    /// </summary>
    /// <param name="amount"></param>
    public void SetAmount(Amount amount)
    {
        Amount = amount;
    }

    #endregion
}
