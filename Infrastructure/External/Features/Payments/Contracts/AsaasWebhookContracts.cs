using System.Text.Json.Serialization;

namespace Infrastructure.External.Features.Payments.Contracts;

/// <summary>
/// Webhook event from Asaas payment notification
/// </summary>
public record AsaasWebhookEvent
{
    [JsonPropertyName("event")]
    public string Event { get; set; } = string.Empty;

    [JsonPropertyName("payment")]
    public PaymentWebhookData? Payment { get; set; }

    [JsonPropertyName("subscription")]
    public SubscriptionWebhookData? Subscription { get; set; }
}

/// <summary>
/// Payment data in webhook
/// </summary>
public record PaymentWebhookData
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("customer")]
    public string Customer { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public decimal Value { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("paymentDate")]
    public DateTime? PaymentDate { get; set; }

    [JsonPropertyName("dueDate")]
    public DateTime? DueDate { get; set; }
}

/// <summary>
/// Subscription data in webhook (for recurring payments)
/// </summary>
public record SubscriptionWebhookData
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("customer")]
    public string Customer { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public decimal Value { get; set; }

    [JsonPropertyName("nextDueDate")]
    public DateTime? NextDueDate { get; set; }
}