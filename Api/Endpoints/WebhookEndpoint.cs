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
                        // Send activation command
                        var command = new Application.Features.Subscriptions.Commands.ActivateDriverSubscription.ActivateDriverSubscriptionCommand(
                            subscriptionId: 0, // Will be found by payment ID
                            asaasPaymentId: webhookEvent.Payment.Id
                        );

                        // Find subscription by payment ID and activate
                        var result = await mediator.Send(command);
                        if (result.IsFailed)
                        {
                            logger.LogWarning("Failed to activate subscription for payment {PaymentId}: {Errors}",
                                webhookEvent.Payment.Id, result.Errors);
                        }
                        else
                        {
                            logger.LogInformation("Subscription activated for payment {PaymentId}", webhookEvent.Payment.Id);
                        }
                    }
                    break;

                case "PAYMENT_EXPIRED":
                    logger.LogWarning("Payment expired: {PaymentId}", webhookEvent.Payment?.Id);
                    // Could implement auto-cancel logic here
                    break;

                case "PAYMENT_CANCELED":
                    logger.LogWarning("Payment canceled: {PaymentId}", webhookEvent.Payment?.Id);
                    // Could implement subscription deactivation here
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
}