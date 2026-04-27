using Domain.Abstractions;
using Domain.Features.Users.Entities;
using Domain.Features.Plans.Entities;
using Domain.Subscriptions.Enums;

namespace Domain.Features.Subscriptions.Entities;

/// <summary>
/// Represents a driver subscription - specific subscription for drivers to receive rides
/// </summary>
public class DriverSubscription : Entity
{

    #region PROPERTIES

    public long UserId { get; protected set; }
    public long PlanId { get; protected set; }
    public User User { get; protected set; } = null!;
    public SubscriptionStatus Status { get; protected set; } = SubscriptionStatus.PendingApproval;
    public Plan Plan { get; protected set; } = null!;
    public DateTime DueDate { get; protected set; }
    public string? PaymentToken { get; protected set; }
    public string? AsaasPaymentId { get; protected set; }
    public string? AsaasPaymentLink { get; protected set; }
    public string? AsaasSubscriptionId { get; protected set; }
    public DateTime? PaidAt { get; protected set; }

    #endregion

    #region CONSTRUCTORS

    private DriverSubscription() { }

    private DriverSubscription(long? id, User user, Plan plan, DateTime? createdAt)
    {
        Id = id ?? 0;
        User = user;
        Plan = plan;
        DueDate = DateTime.Now.AddMonths(1);
        CreatedAt = createdAt ?? DateTime.Now;
        Status = SubscriptionStatus.PendingApproval;
    }

    /// <summary>
    /// Cria uma nova assinatura de driver
    /// </summary>
    /// <param name="user">Motorista</param>
    /// <param name="plan">Plano</param>
    /// <returns></returns>
    public static DriverSubscription Create(
        long? id,
        User user,
        Plan plan,
        DateTime? createdAt)
    {
        return new DriverSubscription(id, user, plan, createdAt);
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
    /// Define o ID do pagamento no Asaas
    /// </summary>
    /// <param name="asaasPaymentId"></param>
    public void SetAsaasPaymentId(string? asaasPaymentId)
        => this.AsaasPaymentId = asaasPaymentId;

    /// <summary>
    /// Define o link de pagamento do Asaas
    /// </summary>
    /// <param name="asaasPaymentLink"></param>
    public void SetAsaasPaymentLink(string? asaasPaymentLink)
        => this.AsaasPaymentLink = asaasPaymentLink;

    /// <summary>
    /// Define o ID da assinatura no Asaas
    /// </summary>
    /// <param name="asaasSubscriptionId"></param>
    public void SetAsaasSubscriptionId(string? asaasSubscriptionId)
        => this.AsaasSubscriptionId = asaasSubscriptionId;

    /// <summary>
    /// Marca a assinatura como paga
    /// </summary>
    public void MarkAsPaid()
    {
        Status = SubscriptionStatus.Active;
        PaidAt = DateTime.Now;
        SetUpdatedAt(DateTime.Now);
    }

    /// <summary>
    /// Verifica se a assinatura está ativa
    /// </summary>
    public bool IsActive() => Status == SubscriptionStatus.Active && DueDate > DateTime.Now;

    #endregion
}