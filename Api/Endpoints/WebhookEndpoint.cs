using Application.Shared.Abstractions;
using Infrastructure.External.Features.Payments.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

/// <summary>
/// Webhook endpoints for external services (Asaas payment notifications)
/// </summary>
public static class WebhookEndpoint
{
    public static void MapWebhookEndpoints(this WebApplication app)
    {
        RouteGroupBuilder webhooks = app.MapGroup("/webhooks")
            .WithTags("Webhooks");

        // Asaas payment webhook
        webhooks.MapPost("/asaas", HandleAsaasWebhook)
            .AllowAnonymous()
            .WithOpenApi();
    }

    private static async Task<IResult> HandleAsaasWebhook(
        [FromBody] AsaasWebhookEvent webhookEvent,
        [FromServices] IMediator mediator,
        [FromServices] ILogger<AsaasWebhookEvent> logger)
    {
        try
        {
            logger.LogInformation("Received Asaas webhook: {EventType}", webhookEvent.Event);

            switch (webhookEvent.Event)
            {
                case "PAYMENT_RECEIVED":
                case "PAYMENT_CONFIRMED":
                    if (webhookEvent.Payment != null)
                    {
                        // Check if this is a passenger ride payment
                        if (IsPassengerRidePayment(webhookEvent.Payment))
                        {
                            await HandlePassengerRidePayment(webhookEvent.Payment, mediator, logger);
                        }
                        else
                        {
                            await HandlePaymentConfirmed(webhookEvent.Payment, mediator, logger);
                        }
                    }
                    break;

                case "PAYMENT_EXPIRED":
                    logger.LogWarning("Payment expired: {PaymentId}", webhookEvent.Payment?.Id);
                    if (webhookEvent.Payment != null)
                        await HandlePaymentInactivation(webhookEvent.Payment, "EXPIRED", mediator, logger);
                    break;

                case "PAYMENT_CANCELED":
                    logger.LogWarning("Payment canceled: {PaymentId}", webhookEvent.Payment?.Id);
                    if (webhookEvent.Payment != null)
                        await HandlePaymentInactivation(webhookEvent.Payment, "CANCELED", mediator, logger);
                    break;

                default:
                    logger.LogInformation("Unhandled webhook event: {EventType}", webhookEvent.Event);
                    break;
            }

            return TypedResults.Ok(new { status = "received" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing Asaas webhook");
            return TypedResults.Problem("Error processing webhook", statusCode: 500);
        }
    }

    /// <summary>
    /// Check if payment is for passenger ride (externalReference is not a subscription)
    /// </summary>
    private static bool IsPassengerRidePayment(PaymentWebhookData payment)
    {
        // If externalReference is present and there's no subscription, it's likely a passenger ride
        return !string.IsNullOrEmpty(payment.ExternalReference) &&
               string.IsNullOrEmpty(payment.Subscription);
    }

    /// <summary>
    /// Handle passenger ride payment confirmation
    /// </summary>
    private static async Task HandlePassengerRidePayment(
        PaymentWebhookData payment,
        IMediator mediator,
        ILogger logger)
    {
        if (string.IsNullOrEmpty(payment.Id))
        {
            logger.LogWarning("Cannot process passenger ride payment - no payment ID");
            return;
        }

        // Update payment status in database
        var command = new Application.Features.Payments.Commands.UpdatePaymentStatus.UpdatePaymentStatusCommand(
            payment.Id,
            payment.Status ?? "CONFIRMED"
        );

        var result = await mediator.Send(command);
        if (result.IsFailed)
        {
            logger.LogWarning("Failed to update passenger payment status for {PaymentId}: {Errors}",
                payment.Id, result.Errors);
        }
        else
        {
            logger.LogInformation("Passenger ride payment status updated: {PaymentId}, Status: {Status}",
                payment.Id, payment.Status);
        }
    }

    /// <summary>
    /// Ativa a assinatura quando um pagamento PIX é confirmado.
    /// Usa o externalReference (ID interno da assinatura Tilt) como chave primária de busca.
    /// Passa o próximo vencimento para atualizar o DueDate em renovações recorrentes.
    /// </summary>
    private static async Task HandlePaymentConfirmed(
        PaymentWebhookData payment,
        IMediator mediator,
        ILogger logger)
    {
        // Extrair ID interno da assinatura (salvo como externalReference ao criar no Asaas)
        long subscriptionId = 0;
        if (!string.IsNullOrEmpty(payment.ExternalReference) &&
            long.TryParse(payment.ExternalReference, out var parsedId))
        {
            subscriptionId = parsedId;
        }

        // Calcular próximo vencimento para renovação (dueDate do payment + 1 mês)
        DateTime? nextDueDate = payment.DueDate.HasValue
            ? payment.DueDate.Value.AddMonths(1)
            : null;

        var command = new Application.Features.Subscriptions.Commands.ActivateSubscription.ActivateSubscriptionCommand(
            subscriptionId: subscriptionId,
            asaasPaymentId: payment.Id,
            dueDate: nextDueDate
        );

        var result = await mediator.Send(command);
        if (result.IsFailed)
        {
            logger.LogWarning("Failed to activate subscription for payment {PaymentId} (subscriptionId={SubscriptionId}): {Errors}",
                payment.Id, subscriptionId, result.Errors);
        }
        else
        {
            logger.LogInformation("Subscription activated for payment {PaymentId} (subscriptionId={SubscriptionId})",
                payment.Id, subscriptionId);
        }
    }

    /// <summary>
    /// Inativa a assinatura quando um pagamento expira ou é cancelado.
    /// Usa o campo subscription (AsaasSubscriptionId) do payload do webhook.
    /// </summary>
    private static async Task HandlePaymentInactivation(
        PaymentWebhookData payment,
        string reason,
        IMediator mediator,
        ILogger logger)
    {
        // Para inativação usamos o AsaasSubscriptionId presente no campo "subscription" do payment
        var asaasSubscriptionId = payment.Subscription;

        if (string.IsNullOrEmpty(asaasSubscriptionId))
        {
            logger.LogWarning("Cannot inactivate subscription — no Asaas subscription ID in payment {PaymentId} (reason={Reason})",
                payment.Id, reason);
            return;
        }

        // Buscar subscription pelo AsaasSubscriptionId
        var query = new Application.Features.Subscriptions.Queries.GetSubscriptionByAsaasId.GetSubscriptionByAsaasIdQuery(asaasSubscriptionId);
        var queryResult = await mediator.Send(query);

        if (queryResult.IsFailed || queryResult.Value == null)
        {
            logger.LogWarning("Cannot inactivate subscription — subscription not found for AsaasSubscriptionId: {AsaasSubscriptionId}",
                asaasSubscriptionId);
            return;
        }

        // Cancelar a assinatura usando o ID interno
        var command = new Application.Features.Subscriptions.Commands.CancelSubscription.CancelSubscriptionCommand(
            subscriptionId: queryResult.Value.Id,
            reason: $"Payment {reason.ToLower()} - {payment.Id}"
        );

        var result = await mediator.Send(command);
        if (result.IsFailed)
        {
            logger.LogWarning("Failed to cancel subscription for AsaasSubscriptionId {AsaasSubscriptionId}: {Errors}",
                asaasSubscriptionId, result.Errors);
        }
        else
        {
            logger.LogInformation("Subscription canceled for AsaasSubscriptionId {AsaasSubscriptionId} (reason: {Reason})",
                asaasSubscriptionId, reason);
        }
    }
}