

using System.Text.Json;
using Application.Shared.Abstractions;
using Domain.Features.Payments.Entities;
using Domain.Features.Subscriptions.Entities;
using Domain.Features.Users.Entities;
using Domain.Shared.Abstractions;
using Infrastructure.External.Features.Payments.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Infrastructure.External.Features.Services;

public class PaymentServices : IPaymentServices
{
    private readonly IUnitOfWork _unitOfWork;
    ILogger<PaymentServices> _logger;
    private readonly IConfiguration _configuration;
    private string BaseUrl { get; set; } = null!;
    private string APIKEY { get; set; } = null!;

    public PaymentServices(
        IUnitOfWork unitOfWork,
        ILogger<PaymentServices> logger,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _configuration = configuration;

        BaseUrl = configuration.GetSection("Security:PaymentUrl").Value!;
        APIKEY = configuration.GetSection("Security:PaymentKey").Value!;
    }

    public async Task<User> CreateUser(long idUser, CancellationToken? cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(idUser);

        if (user == null)
            throw new Exception("user not found");

        if (user != null && !string.IsNullOrEmpty(user.PaymentUserIdentifier))
            throw new Exception("Payment user identifier already exists");

        var paymentUserResponse = await CreateUserRequest(user, cancellationToken);

        user!.SetPaymentUserIdentifier(paymentUserResponse!.Id);
        return user;
    }

    public Task<Payment> CreateToken(long idUser, CancellationToken? cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task<PaymentUserResponse?> CreateUserRequest(User? user, CancellationToken? cancellationToken)
    {
        var client = new RestClient(new RestClientOptions(this.BaseUrl));
        var request = new RestRequest("v3/customers", Method.Post);

        var paymentUser = JsonSerializer.Serialize((PaymentUserRequest)user!);

        request.AddHeader("access_token", $"{APIKEY}");
        request.AddStringBody(paymentUser, DataFormat.Json);

        // The cancellation token comes from the caller. You can still make a call without it.
        RestResponse response = await client.ExecuteAsync(request);
        var paymentUserResponse = JsonSerializer.Deserialize<PaymentUserResponse>(response.Content!);
        return paymentUserResponse;
    }
}