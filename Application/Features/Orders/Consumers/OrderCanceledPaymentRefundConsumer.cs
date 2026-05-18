using System.Threading.Tasks;
using Domain.Features.Orders.Events;
using Application.Shared.Abstractions;
using Application.Features.Orders.Services;
using Domain.Shared.Abstractions;
using Domain.Orders.Enums;
using Domain.Features.Orders.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Application.Features.Orders.Consumers;

/// <summary>
/// Consumer for order canceled payment refund events.
/// This consumer handles the calculation of refund penalties for canceled orders.
/// The penalty is based on the driver's distance ratio at the time of cancellation.
/// </summary>
public class OrderCanceledPaymentRefundConsumer : BackgroundService
{
    private readonly ILogger<OrderCanceledPaymentRefundConsumer> _logger;
    private readonly IConfiguration _configuration;
    private List<IMessageRepository> messageRepositories = [];
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private readonly int _instances = 0;
    private readonly int _delayInterval = 5000;
    private readonly string _queueName = null!;

    protected IConnection Connection { get; set; } = null!;
    protected IChannel SharedChannel { get; set; } = null!;

    private readonly string _className = nameof(OrderCanceledPaymentRefundConsumer);

    public OrderCanceledPaymentRefundConsumer(
        ILogger<OrderCanceledPaymentRefundConsumer> logger,
        IConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        try
        {
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;

            _queueName = _configuration["Infrastructure:OrderCanceledPaymentRefundMessages:queue"]!;
            _instances = int.Parse(_configuration["Infrastructure:OrderCanceledPaymentRefundMessages:consumerIntances"]!);
            _delayInterval = int.Parse(_configuration["Infrastructure:OrderCanceledPaymentRefundMessages:delayInterval"]!);

            Task.Run(async () => await ConfigureStart()).Wait();
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error starting Order Canceled Payment Refund Consumer : Error: {ex}", _className, ex);
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
                repo.ConsumeAsync<OrderCanceledPaymentRefundDomainEvent>(this.SharedChannel, this.ConsumeMessage);
            });

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("[{Classe}] ({intances}) Worker's ativo", nameof(OrderCanceledPaymentRefundConsumer), _instances);
                await Task.Delay(_delayInterval, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error when executing Order Canceled Payment Refund Consumer : Error: {ex}", _className, ex);
            throw;
        }
    }

    private async Task ConsumeMessage(OrderCanceledPaymentRefundDomainEvent orderCanceledPaymentRefundEvent)
    {
        _logger.LogInformation(
            "[{Classe}] Processing order canceled payment refund: OrderId={OrderId}, Amount={Amount}, UserId={UserId}",
            nameof(OrderCanceledPaymentRefundConsumer),
            orderCanceledPaymentRefundEvent.OrderId,
            orderCanceledPaymentRefundEvent.Amount,
            orderCanceledPaymentRefundEvent.UserId);

        try
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
                var penaltyCalculationService = scope.ServiceProvider.GetRequiredService<RefundPenaltyCalculationService>();

                // Validate message payload
                if (orderCanceledPaymentRefundEvent.OrderId <= 0)
                {
                    _logger.LogWarning("[{Classe}] Invalid OrderId in message: {OrderId}",
                        nameof(OrderCanceledPaymentRefundConsumer), orderCanceledPaymentRefundEvent.OrderId);
                    return;
                }

                if (orderCanceledPaymentRefundEvent.Amount <= 0)
                {
                    _logger.LogWarning("[{Classe}] Invalid Amount in message: {Amount}",
                        nameof(OrderCanceledPaymentRefundConsumer), orderCanceledPaymentRefundEvent.Amount);
                    return;
                }

                // Validate order exists and is canceled
                var order = await orderRepository.GetByIdAsync(orderCanceledPaymentRefundEvent.OrderId);
                if (order == null)
                {
                    _logger.LogWarning("[{Classe}] Order not found: {OrderId}",
                        nameof(OrderCanceledPaymentRefundConsumer), orderCanceledPaymentRefundEvent.OrderId);
                    return;
                }

                if (order.Status != OrderStatus.Canceled)
                {
                    _logger.LogWarning("[{Classe}] Order is not in Canceled status: OrderId={OrderId}, Status={Status}",
                        nameof(OrderCanceledPaymentRefundConsumer), order.Id, order.Status);
                    return;
                }

                // If the cancellation was initiated by the driver, no penalty applies and the full amount is refunded.
                if (order.CancelledBy == CancellationInitiator.Driver)
                {
                    _logger.LogInformation(
                        "[{Classe}] Cancellation initiated by driver – no penalty applied. Full refund for OrderId={OrderId}, Amount={Amount}",
                        nameof(OrderCanceledPaymentRefundConsumer),
                        order.Id,
                        order.Amount.Value);
                    // No further processing needed for penalty calculation.
                    return;
                }

                // Check if driver accepted the order
                if (order.DriverId == null)
                {
                    _logger.LogInformation(
                        "[{Classe}] Order canceled before driver acceptance: OrderId={OrderId}, UserId={UserId}, Amount={Amount}",
                        nameof(OrderCanceledPaymentRefundConsumer), order.Id, orderCanceledPaymentRefundEvent.UserId, order.Amount.Value);

                    // No penalty for orders canceled before driver acceptance
                    return;
                }

                // Calculate distance ratio based on order distance
                // For now, use a simple calculation based on driver distance
                // In future iterations, this should use more precise geolocation data
                var distanceRatio = CalculateDistanceRatio(order);

                // Calculate penalty
                var penaltyResult = penaltyCalculationService.CalculatePenalty(order, distanceRatio);

                // Log penalty calculation
                _logger.LogInformation(
                    "[{Classe}] Penalty calculated - OrderId={OrderId}, OriginalAmount={OriginalAmount}, PenaltyPercentage={PenaltyPercentage}%, " +
                    "PenaltyAmount={PenaltyAmount}, RefundAmount={RefundAmount}, DistanceRatio={DistanceRatio}",
                    nameof(OrderCanceledPaymentRefundConsumer),
                    order.Id,
                    penaltyResult.OriginalAmount,
                    penaltyResult.PenaltyPercentage,
                    penaltyResult.PenaltyAmount,
                    penaltyResult.RefundAmount,
                    penaltyResult.DistanceRatio);

                _logger.LogInformation("[{Classe}] Order canceled payment refund processed successfully: OrderId={OrderId}",
                    nameof(OrderCanceledPaymentRefundConsumer), order.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error processing order canceled payment refund - OrderId: {OrderId}, Error: {Exception}",
                _className, orderCanceledPaymentRefundEvent.OrderId, ex);
            throw;
        }
    }

    /// <summary>
    /// Calculates the distance ratio for penalty calculation.
    /// For now, this returns a placeholder ratio.
    /// In future iterations, this should be enhanced with geolocation data.
    /// </summary>
    private decimal CalculateDistanceRatio(Domain.Features.Orders.Entities.Order order)
    {
        // TODO: Implement proper distance ratio calculation using:
        // 1. Driver's location at acceptance time
        // 2. Order's pickup location
        // 3. Total ride distance
        // For now, return 0.5 as a placeholder
        return 0.5m;
    }
}
