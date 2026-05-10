using System.Threading.Tasks;
using Domain.Features.Payments.Contracts;
using Application.Shared.Abstractions;
using Domain.Features.Orders.Events;
using Domain.Shared.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Application.Features.Orders.Consumers;

/// <summary>
/// Consumer for order finished events.
/// This consumer handles the transfer of funds from Tilt wallet to driver.
/// </summary>
public class OrderFinishedConsumer : BackgroundService
{
    private readonly ILogger<OrderFinishedConsumer> _logger;
    private readonly IConfiguration _configuration;
    private List<IMessageRepository> messageRepositories = [];
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private readonly int _instances = 0;
    private readonly int _delayInterval = 5000;
    private readonly string _queueName = null!;

    protected IConnection Connection { get; set; } = null!;
    protected IChannel SharedChannel { get; set; } = null!;

    private readonly string _className = nameof(OrderFinishedConsumer);

    public OrderFinishedConsumer(
        ILogger<OrderFinishedConsumer> logger,
        IConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        try
        {
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;

            _queueName = _configuration["Infrastructure:OrderFinishedMessages:queue"]!;
            _instances = int.Parse(_configuration["Infrastructure:OrderFinishedMessages:consumerIntances"]!);
            _delayInterval = int.Parse(_configuration["Infrastructure:OrderFinishedMessages:delayInterval"]!);

            Task.Run(async () => await ConfigureStart()).Wait();
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error starting Order Finished Consumer : Error: {ex}", _className, ex);
            throw;
        }
    }

    private async Task ConfigureStart()
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var messageRepository = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
            this.Connection = await messageRepository.GetConnectionFactory();
            this.SharedChannel = await messageRepository.StartNewChannel(_queueName);
            for (int i = 0; i < _instances; i++)
            {
                messageRepositories.Add(messageRepository.CreateNewInstance());
            }
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            messageRepositories.ForEach(repo =>
            {
                repo.ConsumeAsync<OrderFinishedDomainEvent>(this.SharedChannel, this.ConsumeMessage);
            });

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("[{Classe}] ({intances}) Worker's ativo", nameof(OrderFinishedConsumer), _instances);
                await Task.Delay(_delayInterval, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error when executing Order Finished Consumer : Error: {ex}", _className, ex);
            throw;
        }
    }

    private async Task ConsumeMessage(OrderFinishedDomainEvent orderFinishedDomainEvent)
    {
        _logger.LogInformation(
            "[{Classe}] Processing order finished: OrderId={OrderId}, Amount={Amount}, DriverId={DriverId}",
            nameof(OrderFinishedConsumer),
            orderFinishedDomainEvent.OrderId,
            orderFinishedDomainEvent.Amount,
            orderFinishedDomainEvent.DriverId);

        try
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var transferService = scope.ServiceProvider.GetRequiredService<IDriverTransferService>();

                // Get driver to retrieve wallet ID
                var driver = await unitOfWork.UserRepository.GetByIdAsync(orderFinishedDomainEvent.DriverId);
                if (driver == null)
                {
                    _logger.LogError(
                        "[{Classe}] Driver not found for Order {OrderId}, DriverId={DriverId}",
                        nameof(OrderFinishedConsumer), orderFinishedDomainEvent.OrderId, orderFinishedDomainEvent.DriverId);
                    return;
                }

                // Check if driver has wallet
                if (string.IsNullOrEmpty(driver.AsaasWalletId))
                {
                    _logger.LogError(
                        "[{Classe}] Driver {DriverId} has no wallet for Order {OrderId}",
                        nameof(OrderFinishedConsumer), orderFinishedDomainEvent.DriverId, orderFinishedDomainEvent.OrderId);
                    return;
                }

                // Execute transfer from Tilt to driver
                var transferId = await transferService.TransferToDriverAsync(
                    orderId: orderFinishedDomainEvent.OrderId,
                    amount: orderFinishedDomainEvent.Amount,
                    driverWalletId: driver.AsaasWalletId,
                    cancellationToken: CancellationToken.None);

                _logger.LogInformation(
                    "[{Classe}] Transfer completed for Order {OrderId}: TransferId={TransferId}",
                    nameof(OrderFinishedConsumer), orderFinishedDomainEvent.OrderId, transferId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "[{Classe}] Error processing transfer for Order {OrderId}: {Exception}",
                nameof(OrderFinishedConsumer), orderFinishedDomainEvent.OrderId, ex);
            throw;
        }
    }
}