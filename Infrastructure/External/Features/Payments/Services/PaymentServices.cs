
using System.Text.Json;
using Application.Shared.Abstractions;
using Domain.Features.Payments.Entities;
using Domain.Features.Subscriptions.Entities;
using Domain.Features.Users.Entities;
using Domain.Shared.Abstractions;
using Infrastructure.External.Features.Payments.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Infrastructure.External.Features.Services;

public class PaymentServices : IPaymentServices
{
    ILogger<PaymentServices> _logger;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private string BaseUrl { get; set; } = null!;
    private string APITOKEN { get; set; } = null!;

    private JsonSerializerOptions SerializationOpt = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public PaymentServices(
        ILogger<PaymentServices> logger,
        IConfiguration configuration,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _configuration = configuration;
        _serviceProvider = serviceProvider;

        BaseUrl = configuration.GetSection("Configurations:PaymentUrl").Value!;
        APITOKEN = configuration.GetSection("Configurations:PaymentToken").Value!;
    }

    public async Task<User> CreateUser(long idUser, CancellationToken? cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var _unitOfWorkScopde = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var user = await _unitOfWorkScopde.UserRepository.GetByIdAsync(idUser) ?? throw new Exception("user not found");
            if (user != null && !string.IsNullOrEmpty(user.PaymentUserIdentifier))
                return user;

            var paymentUserResponse = await CreateUserRequest(user, cancellationToken)
                ?? throw new Exception("Failed to create payment user");

            user!.SetPaymentUserIdentifier(paymentUserResponse.id);
            await _unitOfWorkScopde.UserRepository.UpdateAsync(user);
            await _unitOfWorkScopde.CommitAsync(cancellationToken ?? CancellationToken.None);

            return user;
        }
    }

    public Task<Payment> CreateToken(long idUser, CancellationToken? cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task<PaymentUserResponse?> CreateUserRequest(User? user, CancellationToken? cancellationToken)
    {
        var client = new RestClient(new RestClientOptions(this.BaseUrl));
        var request = new RestRequest("v3/customers", Method.Post);

        var paymentUser = JsonSerializer.Serialize((PaymentUserRequest)user!, SerializationOpt);

        request.AddHeader("access_token", $"{APITOKEN}");
        request.AddHeader("accept", "application/json");
        request.AddHeader("content-type", "application/json");
        request.AddJsonBody(paymentUser);

        // The cancellation token comes from the caller. You can still make a call without it.
        RestResponse response = await client.ExecuteAsync(request);
        var paymentUserResponse = JsonSerializer.Deserialize<PaymentUserResponse>(response.Content!);
        return paymentUserResponse;
    }
}