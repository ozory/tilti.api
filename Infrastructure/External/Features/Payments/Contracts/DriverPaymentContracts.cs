namespace Infrastructure.External.Features.Payments.Contracts;

/// <summary>
/// Request to create a payment link in Asaas
/// </summary>
public record PaymentLinkRequest(
    string name,
    string billingType,
    string chargeType,
    decimal? value,
    string? description,
    DateTime? dueDate,
    int? installmentCount,
    bool? active,
    string? externalReference
);

/// <summary>
/// Response from Asaas payment link creation
/// </summary>
public record PaymentLinkResponse(
    string id,
    string url,
    string status,
    decimal value,
    DateTime? dueDate,
    DateTime? paymentDate
);

/// <summary>
/// Request to create a subscription in Asaas
/// </summary>
public record SubscriptionRequest(
    string customer,
    string billingType,
    decimal value,
    string nextDueDate,
    string cycle,
    string? description = null,
    DateTime? endDate = null,
    int? maxPayments = null,
    string? externalReference = null
);

/// <summary>
/// Response from Asaas subscription creation
/// </summary>
public record SubscriptionResponse(
    string id,
    string? @object,
    string? dateCreated,
    string customer,
    string? paymentLink,
    string billingType,
    string cycle,
    decimal value,
    string? nextDueDate,
    string? endDate,
    string? description,
    string status,
    bool deleted
);

/// <summary>
/// Request to create a wallet (account) in Asaas for driver
/// </summary>
public record WalletRequest(
    string name,
    string cpfCnpj,
    string email,
    string? phone,
    string? mobilePhone,
    string address,
    string addressNumber,
    string complement,
    string province,
    string city,
    string state,
    string country,
    string postalCode
);

/// <summary>
/// Response from Asaas wallet creation
/// </summary>
public record WalletResponse(string id, string name, string email, string status);