using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Orders.Commands.CloseOrder;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Consumers;

public class CloseExpiredOrdersConsumer : BackgroundService
{
    private readonly ILogger<CloseExpiredOrdersConsumer> _logger;
    private readonly IConfiguration _configuration;
    private readonly int _delayInterval = 50000;  // 5 minutos
    private readonly int _expiredMinutes = 5;  // 5 minutos 
    private readonly IServiceProvider _serviceProvider;
    private readonly string className = nameof(CloseExpiredOrdersConsumer)!;

    public CloseExpiredOrdersConsumer(
        ILogger<CloseExpiredOrdersConsumer> logger,
        IConfiguration configuration,
        IMediator mediator,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        try
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _delayInterval = int.Parse(_configuration["Infrastructure:CloseExpiredOrders:delayInterval"]!);
            _expiredMinutes = int.Parse(_configuration["Infrastructure:CloseExpiredOrders:expiredMinutes"]!);
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error starting Close Expired Orders Consumer : Error: {ex}", className, ex);
            throw;
        }

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("[{Classe}] Worker's ativo", nameof(CloseExpiredOrdersConsumer));

                using (var scope = _serviceProvider.CreateScope())
                {
                    var _mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var command = new CloseExpiredOrdersCommand(DateTime.Now.AddMinutes(-_expiredMinutes));
                    await _mediator.Send(command);
                }

                await Task.Delay(_delayInterval, stoppingToken);
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error when executing Close Expired Orders Consumer : Error: {ex}", className, ex);
            throw;
        }

    }
}
