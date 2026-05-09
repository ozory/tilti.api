using Domain.Abstractions;
using Domain.Features.Orders.Entities;
using Domain.Features.Payments.Enums;
using Domain.Features.Users.Entities;
using Domain.ValueObjects;

namespace Domain.Features.Payments.Entities;

public class DriverTransfer : Entity
{
    #region PROPERTIES

    public long OrderId { get; protected set; }
    public Order Order { get; protected set; } = null!;

    public long DriverId { get; protected set; }
    public User Driver { get; protected set; } = null!;

    public Amount Amount { get; protected set; } = null!;

    public TransferStatus Status { get; protected set; } = TransferStatus.Pending;

    public string? AsaasTransferId { get; protected set; }

    public DateTime? CompletedAt { get; protected set; }
    public DateTime? FailedAt { get; protected set; }

    public string? ErrorMessage { get; protected set; }

    #endregion

    #region CONSTRUCTORS

    protected DriverTransfer()
    {
    }

    public static DriverTransfer Create(
        long? id,
        Order order,
        User driver,
        Amount amount)
    {
        var transfer = new DriverTransfer
        {
            Id = id ?? 0,
            Order = order,
            OrderId = order.Id,
            Driver = driver,
            DriverId = driver.Id,
            Amount = amount,
            Status = TransferStatus.Pending,
            CreatedAt = DateTime.Now
        };

        return transfer;
    }

    #endregion

    #region METHODS

    public void SetAsaasTransferId(string transferId)
    {
        AsaasTransferId = transferId;
    }

    public void MarkAsCompleted()
    {
        Status = TransferStatus.Completed;
        CompletedAt = DateTime.Now;
    }

    public void MarkAsFailed(string errorMessage)
    {
        Status = TransferStatus.Failed;
        FailedAt = DateTime.Now;
        ErrorMessage = errorMessage;
    }

    #endregion
}