using System.Text.Json;
using Application.Shared.Abstractions;
using Domain.Features.Subscriptions.Entities;
using Domain.Subscriptions.Enums;
using Domain.Shared.Abstractions;
using Infrastructure.External.Features.Payments.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Infrastructure.External.Features.Payments.Services;

/// <summary>
/// Service for subscription payment operations with Asaas (unified for Driver and Passenger)
/// </summary>
public class DriverPaymentService : ISubscriptionPaymentService
{
    private readonly ILogger<DriverPaymentService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _baseUrl;
    private readonly string _apiToken;

    private JsonSerializerOptions SerializationOpt = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public DriverPaymentService(
        ILogger<DriverPaymentService> logger,
        IConfiguration configuration,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _configuration = configuration;
        _serviceProvider = serviceProvider;

        _baseUrl = configuration.GetSection("Configurations:PaymentUrl").Value!;
        _apiToken = configuration.GetSection("Configurations:PaymentToken").Value!;
    }

    public async Task<string> CreatePaymentLinkAsync(Subscription subscription, SubscriptionType subscriptionType, CancellationToken cancellationToken = default)
    {
        var client = new RestClient(new RestClientOptions(_baseUrl));
        var request = new RestRequest("v3/paymentLinks", Method.Post);

        var paymentLinkRequest = new PaymentLinkRequest(
            name: $"Assinatura {subscription.Plan.Name.Value}",
            billingType: "PIX",
            chargeType: "RECURRENT",
            value: subscription.Plan.Amount.Value,
            description: $"Assinatura Plano {GetSubscriptionTypeDescription(subscriptionType)} - {subscription.Plan.Name.Value}",
            dueDate: DateTime.Now.AddDays(3),
            installmentCount: 1,
            active: true,
            externalReference: subscription.Id.ToString()
        );

        var jsonBody = JsonSerializer.Serialize(paymentLinkRequest, SerializationOpt);

        request.AddHeader("access_token", _apiToken);
        request.AddHeader("accept", "application/json");
        request.AddHeader("content-type", "application/json");
        request.AddJsonBody(jsonBody);

        var response = await client.ExecuteAsync(request, cancellationToken);

        if (!response.IsSuccessful)
        {
            _logger.LogError("Failed to create payment link: {Error}", response.Content);
            throw new Exception($"Failed to create payment link: {response.Content}");
        }

        var paymentLinkResponse = JsonSerializer.Deserialize<PaymentLinkResponse>(response.Content!, SerializationOpt)
            ?? throw new Exception("Failed to deserialize payment link response");

        return paymentLinkResponse.url;
    }

    public async Task<(string subscriptionId, string paymentLink)> CreateSubscriptionAsync(Subscription subscription, SubscriptionType subscriptionType, CancellationToken cancellationToken = default)
    {
        var client = new RestClient(new RestClientOptions(_baseUrl));
        var request = new RestRequest("v3/subscriptions", Method.Post);

        var subscriptionRequest = new SubscriptionRequest(
            customer: subscription.User.PaymentUserIdentifier ?? throw new Exception("User payment identifier not found"),
            billingType: "PIX",
            value: subscription.Plan.Amount.Value,
            nextDueDate: DateTime.Now.ToString("yyyy-MM-dd"),
            cycle: "MONTHLY",
            description: $"Assinatura Plano {GetSubscriptionTypeDescription(subscriptionType)} - {subscription.Plan.Name.Value}",
            endDate: null,
            maxPayments: null,
            externalReference: subscription.Id.ToString()
        );

        var jsonBody = JsonSerializer.Serialize(subscriptionRequest, SerializationOpt);

        request.AddHeader("access_token", _apiToken);
        request.AddHeader("accept", "application/json");
        request.AddHeader("content-type", "application/json");
        request.AddJsonBody(jsonBody);

        var response = await client.ExecuteAsync(request, cancellationToken);

        if (!response.IsSuccessful)
        {
            _logger.LogError("Failed to create subscription: {Error}", response.Content);
            throw new Exception($"Failed to create subscription: {response.Content}");
        }

        var subscriptionResponse = JsonSerializer.Deserialize<SubscriptionResponse>(response.Content!, SerializationOpt)
            ?? throw new Exception("Failed to deserialize subscription response");

        return (subscriptionResponse.id, subscriptionResponse.paymentLink ?? "");
    }

    private static string GetSubscriptionTypeDescription(SubscriptionType subscriptionType)
    {
        return subscriptionType switch
        {
            SubscriptionType.Driver => "Motorista",
            SubscriptionType.Passenger => "Passageiro",
            _ => "Desconhecido"
        };
    }

    public async Task<bool> CheckPaymentStatusAsync(string asaasPaymentId, CancellationToken cancellationToken = default)
    {
        var client = new RestClient(new RestClientOptions(_baseUrl));
        var request = new RestRequest($"v3/payments/{asaasPaymentId}", Method.Get);

        request.AddHeader("access_token", _apiToken);
        request.AddHeader("accept", "application/json");

        var response = await client.ExecuteAsync(request, cancellationToken);

        if (!response.IsSuccessful)
        {
            _logger.LogError("Failed to check payment status: {Error}", response.Content);
            return false;
        }

        // Parse response to check status - looking for "CONFIRMED" status
        var content = response.Content?.ToLower() ?? "";
        return content.Contains("confirmed");
    }

    public async Task<string> CreateDriverWalletAsync(long userId, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var user = await unitOfWork.UserRepository.GetByIdAsync(userId)
            ?? throw new Exception("User not found");

        // Check if user already has a wallet
        if (!string.IsNullOrEmpty(user.AsaasWalletId))
        {
            return user.AsaasWalletId;
        }

        var client = new RestClient(new RestClientOptions(_baseUrl));
        var request = new RestRequest("v3/accounts", Method.Post);

        var walletRequest = new WalletRequest(
            name: user.Name.Value,
            cpfCnpj: user.Document.Value,
            email: user.Email.Value,
            phone: null,
            mobilePhone: null,
            address: "",
            addressNumber: "",
            complement: "",
            province: "",
            city: "",
            state: "",
            country: "Brasil",
            postalCode: ""
        );

        var jsonBody = JsonSerializer.Serialize(walletRequest, SerializationOpt);

        request.AddHeader("access_token", _apiToken);
        request.AddHeader("accept", "application/json");
        request.AddHeader("content-type", "application/json");
        request.AddJsonBody(jsonBody);

        var response = await client.ExecuteAsync(request, cancellationToken);

        if (!response.IsSuccessful)
        {
            _logger.LogError("Failed to create driver wallet: {Error}", response.Content);
            throw new Exception($"Failed to create driver wallet: {response.Content}");
        }

        var walletResponse = JsonSerializer.Deserialize<WalletResponse>(response.Content!, SerializationOpt)
            ?? throw new Exception("Failed to deserialize wallet response");

        // Update user with wallet ID
        user.SetAsaasWalletId(walletResponse.id);
        await unitOfWork.UserRepository.UpdateAsync(user);
        await unitOfWork.CommitAsync(cancellationToken);

        return walletResponse.id;
    }
}