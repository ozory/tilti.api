
using Domain.Abstractions;
using DomainUser = Domain.Features.User.Entities.User;
using Domain.Plans.Entities;
using Domain.Subscriptions.Enums;
using Domain.ValueObjects;
using FluentResults;
using FluentValidation.Results;

namespace Domain.Features.Subscription.Entities;

/// <summary>
/// Representa uma assinatura
/// </summary>
public class Subscription : Entity<Subscription>
{
    public DomainUser User { get; private set; } = null!;
    public SubscriptionStatus Status { get; private set; } = SubscriptionStatus.Active;
    public Plan Plan { get; private set; } = null!;
    public DateTime DueDate { get; private set; }

    private Subscription(DomainUser user, Plan plan)
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
    public static Result<Subscription> Create(DomainUser user, Plan plan)
    {
        return new Subscription(user, plan);
    }

    /// <summary>
    /// Altera o status da assinatura
    /// </summary>
    /// <param name="status"></param> 
    public void ChangeStatus(SubscriptionStatus status)
    {
        if (status != this.Status)
        {
            this.Status = status;
            UpdatedAt = DateTime.Now;
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
}
