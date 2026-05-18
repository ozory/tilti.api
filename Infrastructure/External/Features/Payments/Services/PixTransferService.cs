using System.Text.Json;
using Application.Shared.Abstractions;
using Domain.Features.Orders.Repository;
using Domain.Features.Users.Repository;
using Infrastructure.External.Features.Payments.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Infrastructure.External.Features.Payments.Services;

/// <summary>
/// Service for transferring funds from Tilt wallet to customer wallet via PIX (Asaas)
/// </summary>
public class PixTransferService : IPixTransferService
{
    private readonly ILogger<PixTransferService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;
    private readonly string _baseUrl;
    private readonly string _apiToken;

    private readonly JsonSerializerOptions _serializationOpt = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public PixTransferService(
        ILogger<PixTransferService> logger,
        IConfiguration configuration,
        IOrderRepository orderRepository,
        IUserRepository userRepository)
    {
        _logger = logger;
        _configuration = configuration;
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _baseUrl = configuration.GetSection("Configurations:PaymentUrl").Value!;
        _apiToken = configuration.GetSection("Configurations:PaymentToken").Value!;
    }

    /// <summary>
    /// Transfer refund amount from Tilt wallet to customer wallet via PIX
    /// </summary>
    public async Task<PixTransferResponse> TransferRefundAsync(
        long orderId,
        decimal amount,
        string customerWalletId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[{ClassName}] Initiating PIX refund transfer for OrderId: {OrderId}, Amount: {Amount}, CustomerWallet: {CustomerWalletId}",
            nameof(PixTransferService), orderId, amount, customerWalletId);

        var client = new RestClient(new RestClientOptions(_baseUrl));
        var request = new RestRequest("v3/transfers", Method.Post);

        var transferRequest = new PixTransferRequest(
            value: amount.ToString("F2"),
            pixTransferId: null,
            walletId: customerWalletId,
            externalReference: $"refund-{orderId}"
        );

        request.AddHeader("access_token", _apiToken);
        request.AddHeader("accept", "application/json");
        request.AddHeader("content-type", "application/json");
        request.AddJsonBody(transferRequest);

        RestResponse response = await client.ExecuteAsync(request, cancellationToken);

        if (!response.IsSuccessful)
        {
            _logger.LogError(
                "[{ClassName}] PIX transfer failed for OrderId: {OrderId}. Status: {Status}, Error: {Error}",
                nameof(PixTransferService), orderId, response.StatusCode, response.ErrorMessage);
            throw new Exception($"PIX transfer failed: {response.ErrorMessage}");
        }

        var transferResponse = JsonSerializer.Deserialize<PixTransferResponse>(response.Content!, _serializationOpt)
            ?? throw new Exception("Failed to deserialize PIX transfer response");

        _logger.LogInformation(
            "[{ClassName}] PIX transfer successful for OrderId: {OrderId}. TransferId: {TransferId}",
            nameof(PixTransferService), orderId, transferResponse.id);

        return transferResponse;
    }
}

