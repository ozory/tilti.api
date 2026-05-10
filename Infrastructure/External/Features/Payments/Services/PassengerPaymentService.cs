using System.Text.Json;
using Domain.Features.Payments.Contracts;
using Application.Shared.Abstractions;
using Domain.Features.Users.Entities;
using Domain.Features.Users.Repository;
using Domain.Shared.Abstractions;
using Infrastructure.External.Features.Payments.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Infrastructure.External.Features.Payments.Services;

/// <summary>
/// Service for passenger payment operations with Asaas
/// </summary>
public class PassengerPaymentService : IPassengerPaymentService
{
    private readonly ILogger<PassengerPaymentService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _baseUrl;
    private readonly string _apiToken;

    private JsonSerializerOptions SerializationOpt = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public PassengerPaymentService(
        ILogger<PassengerPaymentService> logger,
        IConfiguration configuration,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _baseUrl = configuration.GetSection("Configurations:PaymentUrl").Value!;
        _apiToken = configuration.GetSection("Configurations:PaymentToken").Value!;
    }

    /// <summary>
    /// Create or retrieve existing wallet for passenger in Asaas
    /// </summary>
    public async Task<string> CreateOrGetWalletAsync(long userId, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId) ?? throw new Exception("User not found");

        // Check if user already has a wallet
        if (!string.IsNullOrEmpty(user?.AsaasWalletId))
        {
            _logger.LogInformation("[{ClassName}] User {UserId} already has wallet: {WalletId}",
                nameof(PassengerPaymentService), userId, user.AsaasWalletId);
            return user.AsaasWalletId;
        }

        // Create wallet in Asaas
        var client = new RestClient(new RestClientOptions(_baseUrl));
        var request = new RestRequest("v3/accounts", Method.Post);

        var walletRequest = new WalletRequest(
            name: user?.Name.Value!,
            cpfCnpj: user?.Document.Value!,
            email: user?.Email?.Value!,
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
            _logger.LogError("[{ClassName}] Failed to create passenger wallet: {Error}",
                nameof(PassengerPaymentService), response.Content);
            throw new Exception($"Failed to create passenger wallet: {response.Content}");
        }

        var walletResponse = JsonSerializer.Deserialize<WalletResponse>(response.Content!, SerializationOpt)
            ?? throw new Exception("Failed to deserialize wallet response");

        // Update user with wallet ID
        user!.SetAsaasWalletId(walletResponse.id);
        await _unitOfWork.UserRepository.UpdateAsync(user);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("[{ClassName}] Created wallet for user {UserId}: {WalletId}",
            nameof(PassengerPaymentService), userId, walletResponse.id);

        return walletResponse.id;
    }

    /// <summary>
    /// Create a PIX payment link in Asaas for passenger ride
    /// </summary>
    public async Task<(string paymentId, string pixLink, string qrCode)> CreatePaymentAsync(
        long orderId,
        decimal amount,
        string customerWalletId,
        DateTime dueDate,
        CancellationToken cancellationToken = default)
    {
        var client = new RestClient(new RestClientOptions(_baseUrl));
        var request = new RestRequest("v3/paymentLinks", Method.Post);

        var paymentRequest = new PaymentLinkRequest(
            name: $"Corrida {orderId}",
            billingType: "PIX",
            chargeType: "DETACHED",  // Single payment, not recurrent
            value: amount,
            description: $"Pagamento de corrida - Order {orderId}",
            dueDate: dueDate,
            installmentCount: 1,
            active: true,
            externalReference: orderId.ToString()
        );

        var jsonBody = JsonSerializer.Serialize(paymentRequest, SerializationOpt);

        request.AddHeader("access_token", _apiToken);
        request.AddHeader("accept", "application/json");
        request.AddHeader("content-type", "application/json");
        request.AddJsonBody(jsonBody);

        var response = await client.ExecuteAsync(request, cancellationToken);

        if (!response.IsSuccessful)
        {
            _logger.LogError("[{ClassName}] Failed to create payment link: {Error}",
                nameof(PassengerPaymentService), response.Content);
            throw new Exception($"Failed to create payment link: {response.Content}");
        }

        var paymentResponse = JsonSerializer.Deserialize<PaymentLinkResponse>(response.Content!, SerializationOpt)
            ?? throw new Exception("Failed to deserialize payment link response");

        _logger.LogInformation("[{ClassName}] Created payment link for order {OrderId}: {PaymentId}",
            nameof(PassengerPaymentService), orderId, paymentResponse.id);

        return (paymentResponse.id, paymentResponse.url, "");  // qrCode not available in PaymentLinkResponse
    }

    /// <summary>
    /// Check payment status in Asaas
    /// </summary>
    public async Task<bool> CheckPaymentStatusAsync(string asaasPaymentId, CancellationToken cancellationToken = default)
    {
        var client = new RestClient(new RestClientOptions(_baseUrl));
        var request = new RestRequest($"v3/payments/{asaasPaymentId}", Method.Get);

        request.AddHeader("access_token", _apiToken);
        request.AddHeader("accept", "application/json");

        var response = await client.ExecuteAsync(request, cancellationToken);

        if (!response.IsSuccessful)
        {
            _logger.LogError("[{ClassName}] Failed to check payment status: {Error}",
                nameof(PassengerPaymentService), response.Content);
            return false;
        }

        // Parse response to check status - looking for "CONFIRMED" status
        var content = response.Content?.ToLower() ?? "";
        var isConfirmed = content.Contains("\"status\":\"confirmed\"");

        _logger.LogInformation("[{ClassName}] Payment {PaymentId} status check: {IsConfirmed}",
            nameof(PassengerPaymentService), asaasPaymentId, isConfirmed);

        return isConfirmed;
    }

    /// <summary>
    /// Wallet response from Asaas API
    /// </summary>
    private record WalletResponse(string id);
}
