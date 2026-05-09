using System.Text.Json;
using Application.Features.Payments.Contracts;
using Domain.Features.Orders.Repository;
using Domain.Features.Payments.Entities;
using Domain.Features.Payments.Repository;
using Domain.Features.Users.Repository;
using Domain.Shared.Abstractions;
using Infrastructure.External.Features.Payments.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Infrastructure.External.Features.Payments.Services;

/// <summary>
/// Service for transferring funds from Tilt wallet to driver wallet via Asaas
/// </summary>
public class DriverTransferService : IDriverTransferService
{
    private readonly ILogger<DriverTransferService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;
    private readonly string _baseUrl;
    private readonly string _apiToken;

    private readonly JsonSerializerOptions _serializationOpt = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public DriverTransferService(
        ILogger<DriverTransferService> logger,
        IConfiguration configuration,
        IUnitOfWork unitOfWork,
        IOrderRepository orderRepository,
        IUserRepository userRepository)
    {
        _logger = logger;
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _baseUrl = configuration.GetSection("Configurations:PaymentUrl").Value!;
        _apiToken = configuration.GetSection("Configurations:PaymentToken").Value!;
    }

    /// <summary>
    /// Transfer funds from Tilt wallet to driver wallet
    /// </summary>
    public async Task<string> TransferToDriverAsync(
        long orderId,
        decimal amount,
        string driverWalletId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[{ClassName}] Initiating transfer for Order {OrderId}, Amount: {Amount}, DriverWallet: {DriverWalletId}",
            nameof(DriverTransferService), orderId, amount, driverWalletId);

        // Get order and driver
        var order = await _orderRepository.GetByIdAsync(orderId)
            ?? throw new Exception($"Order {orderId} not found");

        if (!order.DriverId.HasValue)
            throw new Exception($"Order {orderId} has no driver assigned");

        var driver = await _userRepository.GetByIdAsync(order.DriverId.Value)
            ?? throw new Exception($"Driver {order.DriverId} not found");

        // Create transfer record
        var transfer = DriverTransfer.Create(null, order, driver, new Domain.ValueObjects.Amount(amount));
        await _unitOfWork.DriverTransferRepository.SaveAsync(transfer);

        var client = new RestClient(new RestClientOptions(_baseUrl));
        var request = new RestRequest("v3/transfers", Method.Post);

        var transferRequest = new TransferRequest(
            value: amount.ToString("F2"),
            pixTransferId: null,
            walletId: driverWalletId,
            externalReference: orderId.ToString()
        );

        var jsonBody = JsonSerializer.Serialize(transferRequest, _serializationOpt);

        request.AddHeader("access_token", _apiToken);
        request.AddHeader("accept", "application/json");
        request.AddHeader("content-type", "application/json");
        request.AddJsonBody(jsonBody);

        var response = await client.ExecuteAsync(request, cancellationToken);

        if (!response.IsSuccessful)
        {
            transfer.MarkAsFailed(response.Content ?? "Unknown error");
            await _unitOfWork.DriverTransferRepository.UpdateAsync(transfer);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogError(
                "[{ClassName}] Failed to create transfer for Order {OrderId}: {Error}",
                nameof(DriverTransferService), orderId, response.Content);
            throw new Exception($"Failed to create transfer: {response.Content}");
        }

        var transferResponse = JsonSerializer.Deserialize<TransferResponse>(response.Content!, _serializationOpt)
            ?? throw new Exception("Failed to deserialize transfer response");

        transfer.SetAsaasTransferId(transferResponse.id);
        transfer.MarkAsCompleted();
        await _unitOfWork.DriverTransferRepository.UpdateAsync(transfer);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation(
            "[{ClassName}] Transfer created for Order {OrderId}: {TransferId}",
            nameof(DriverTransferService), orderId, transferResponse.id);

        return transferResponse.id;
    }
}