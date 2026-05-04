
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

    public long UserId { get; protected set; }
    public long PlanId { get; protected set; }
    public User User { get; protected set; } = null!;
    public SubscriptionStatus Status { get; protected set; } = SubscriptionStatus.PendingApproval;
    public SubscriptionType SubscriptionType { get; protected set; } = SubscriptionType.Driver; // NEW: Default to Driver for backward compatibility
    public Plan Plan { get; protected set; } = null!;
    public DateTime DueDate { get; protected set; }
    public string? PaymentToken { get; protected set; }
    public string? AsaasPaymentId { get; protected set; }
    public string? AsaasPaymentLink { get; protected set; }
    public string? AsaasSubscriptionId { get; protected set; }
    public DateTime? PaidAt { get; protected set; }

    #endregion

    #region CONSTRUCTORS

    private Subscription() { }

    private Subscription(long? id, User user, Plan plan, SubscriptionType subscriptionType, DateTime? createdAt)
    {
        Id = id ?? 0;
        User = user;
        Plan = plan;
        SubscriptionType = subscriptionType; // NEW: Set subscription type
        DueDate = DateTime.Now.AddMonths(1);
        CreatedAt = createdAt ?? DateTime.Now;
        Status = SubscriptionStatus.PendingApproval;
    }

    /// <summary>
    /// Cria uma nova assinatura
    /// </summary>
    /// <param name="user">Cliente</param>
    /// <param name="plan">Plano</param>
    /// <param name="subscriptionType">Tipo de assinatura (Driver ou Passenger)</param>
    /// <returns></returns>
    public static Subscription Create(
        long? id,
        User user,
        Plan plan,
        DateTime? createdAt,
        SubscriptionType subscriptionType = SubscriptionType.Driver // NEW: Accept SubscriptionType parameter
        )
    {
        return new Subscription(id, user, plan, subscriptionType, createdAt);
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="paymentToken"></param>
    public void SetPaymentToken(string? paymentToken)
        => this.PaymentToken = paymentToken;

    /// <summary>
    /// Define o tipo de assinatura
    /// </summary>
    /// <param name="subscriptionType"></param>
    public void SetSubscriptionType(SubscriptionType subscriptionType)
        => this.SubscriptionType = subscriptionType;

    /// <summary>
    /// Define o ID de pagamento Asaas
    /// </summary>
    /// <param name="asaasPaymentId"></param>
    public void SetAsaasPaymentId(string? asaasPaymentId)
        => this.AsaasPaymentId = asaasPaymentId;

    /// <summary>
    /// Define o link de pagamento Asaas
    /// </summary>
    /// <param name="asaasPaymentLink"></param>
    public void SetAsaasPaymentLink(string? asaasPaymentLink)
        => this.AsaasPaymentLink = asaasPaymentLink;

    /// <summary>
    /// Define o ID de assinatura Asaas
    /// </summary>
    /// <param name="asaasSubscriptionId"></param>
    public void SetAsaasSubscriptionId(string? asaasSubscriptionId)
        => this.AsaasSubscriptionId = asaasSubscriptionId;

    /// <summary>
    /// Define a data de pagamento
    /// </summary>
    /// <param name="paidAt"></param>
    public void SetPaidAt(DateTime? paidAt)
        => this.PaidAt = paidAt;

    /// <summary>
    /// Marca a assinatura como paga e altera o status para Active
    /// </summary>
    public void MarkAsPaid()
    {
        this.PaidAt = DateTime.Now;
        this.Status = SubscriptionStatus.Active;
        SetUpdatedAt(DateTime.Now);
    }

    #endregion
}
