
using Domain.Abstractions;
using Domain.Features.Users.Entities;
using Domain.Features.Plans.Entities;
using Domain.Subscriptions.Enums;

namespace Domain.Features.Subscriptions.Entities;

/// <summary>
/// Represents a subscription
/// </summary>
public class Subscription : Entity
{

    #region PROPERTIES

    public User User { get; private set; } = null!;
    public SubscriptionStatus Status { get; private set; } = SubscriptionStatus.Active;
    public Plan Plan { get; private set; } = null!;
    public DateTime DueDate { get; private set; }

    #endregion

    #region CONSTRUCTORS

    private Subscription(long? id, User user, Plan plan, DateTime? createdAt)
    {
        Id = id ?? 0;
        User = user;
        Plan = plan;
        DueDate = DateTime.Now.AddMonths(1);
        CreatedAt = createdAt ?? DateTime.Now;
        Status = SubscriptionStatus.PendingApproval;
    }

    /// <summary>
    /// Cria uma nova assinatura
    /// </summary>
    /// <param name="user">Cliente</param>
    /// <param name="plan">Plano</param>
    /// <returns></returns>
    public static Subscription Create(
        long? id,
        User user,
        Plan plan,
        DateTime? createdAt)
    {
        return new Subscription(id, user, plan, createdAt);
    }

    #endregion

    #region  METHODS

    /// <summary>
    /// Altera o status da assinatura
    /// </summary>
    /// <param name="status"></param> 
    public void SetStatus(SubscriptionStatus status)
    {
        if (status != this.Status)
        {
            this.Status = status;
            SetUpdatedAt(DateTime.Now);
        }
        return;
    }

    /// <summary>
    /// Atualiza data de Cobrança
    /// </summary>
    /// <param name="newDueDate">Nova data</param>
    /// <returns></returns>
    public void SetDueDate(DateTime newDueDate)
    {
        DueDate = newDueDate;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="date"></param>
    public void SetUpdatedAt(DateTime date) => this.UpdatedAt = date;

    #endregion
}
