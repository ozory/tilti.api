
using Domain.Abstractions;
using Domain.Features.Users.Entities;
using Domain.Features.Plans.Entities;
using Domain.Subscriptions.Enums;
using Domain.ValueObjects;
using FluentResults;
using FluentValidation.Results;

namespace Domain.Features.Subscriptions.Entities;

/// <summary>
/// Represents a subscription
/// </summary>
public class Subscription : Entity<Subscription>
{

    #region PROPERTIES

    public User User { get; private set; } = null!;
    public SubscriptionStatus Status { get; private set; } = SubscriptionStatus.Active;
    public Plan Plan { get; private set; } = null!;
    public DateTime DueDate { get; private set; }

    #endregion

    #region CONSTRUCTORS

    private Subscription(User user, Plan plan)
    {
        User = user;
        Plan = plan;
        DueDate = DateTime.Now.AddMonths(1);
    }

    /// <summary>
    /// Cria uma nova assinatura
    /// </summary>
    /// <param name="user">Cliente</param>
    /// <param name="plan">Plano</param>
    /// <returns></returns>
    public static Subscription Create(User user, Plan plan)
    {
        return new Subscription(user, plan);
    }

    #endregion

    #region  METHODS

    /// <summary>
    /// Altera o status da assinatura
    /// </summary>
    /// <param name="status"></param> 
    public void ChangeStatus(SubscriptionStatus status)
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
    public void UpdateDueDate(DateTime newDueDate)
    {
        if (newDueDate < DateTime.Now) AddError("A data não pode ser menor");
        DueDate = newDueDate;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="date"></param>
    public void SetUpdatedAt(DateTime date) => this.UpdatedAt = date;

    #endregion
}
