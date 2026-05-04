using Application.Shared.Abstractions;
using Domain.Features.Subscriptions.Entities;
using Domain.Features.Subscriptions.Repository;
using Domain.Shared.Abstractions;
using Domain.Subscriptions.Enums;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Features.Subscriptions.Commands.ActivateSubscription;

/// <summary>
/// Handler for ActivateSubscriptionCommand.
/// Ativado pelo webhook do Asaas quando um pagamento PIX é confirmado.
/// Estratégia de busca (em ordem de prioridade):
///   1. subscriptionId > 0  → busca direta pelo ID interno (vindo do externalReference do webhook)
///   2. asaasPaymentId      → fallback, busca pelo ID do pagamento Asaas
/// </summary>
public class ActivateSubscriptionCommandHandler : ICommandHandler<ActivateSubscriptionCommand, bool>
{
    private readonly ISubscriptionRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ActivateSubscriptionCommandHandler> _logger;
    private readonly string className = nameof(ActivateSubscriptionCommandHandler);

    public ActivateSubscriptionCommandHandler(
        ISubscriptionRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<ActivateSubscriptionCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(ActivateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "[{className}] Activating subscription — subscriptionId={SubscriptionId}, asaasPaymentId={PaymentId}",
            className, request.subscriptionId, request.asaasPaymentId);

        try
        {
            Subscription? subscription;

            // Prioridade 1: ID interno da assinatura (externalReference do webhook Asaas)
            if (request.subscriptionId > 0)
            {
                subscription = await _repository.GetByIdAsync(request.subscriptionId);
            }
            // Prioridade 2: fallback por ID do pagamento Asaas
            else if (!string.IsNullOrEmpty(request.asaasPaymentId))
            {
                subscription = await _repository.GetByAsaasPaymentId(request.asaasPaymentId);
            }
            else
            {
                return Result.Fail("Either subscriptionId or asaasPaymentId must be provided");
            }

            if (subscription == null)
            {
                _logger.LogWarning("[{className}] Subscription not found — subscriptionId={SubscriptionId}, asaasPaymentId={PaymentId}",
                    className, request.subscriptionId, request.asaasPaymentId);
                return Result.Fail("Subscription not found");
            }

            // Salvar AsaasPaymentId caso ainda não esteja preenchido
            if (!string.IsNullOrEmpty(request.asaasPaymentId) && string.IsNullOrEmpty(subscription.AsaasPaymentId))
                subscription.SetAsaasPaymentId(request.asaasPaymentId);

            // Ativar e marcar como pago
            subscription.MarkAsPaid();

            // Renovação: atualizar data de vencimento se fornecida
            if (request.dueDate.HasValue)
                subscription.SetDueDate(request.dueDate.Value);

            await _repository.UpdateAsync(subscription);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "[{className}] Subscription activated successfully — Id={Id}, DueDate={DueDate}",
                className, subscription.Id, subscription.DueDate);

            return Result.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error activating subscription: {Error}", className, ex.Message);
            return Result.Fail($"Error activating subscription: {ex.Message}");
        }
    }
}