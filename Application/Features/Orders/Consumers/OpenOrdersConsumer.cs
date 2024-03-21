using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Consumers;

public class OpenOrdersConsumer : BackgroundService
{
    private readonly ILogger<OpenOrdersConsumer> _logger;
    private readonly IConfiguration _configuration;
    private readonly int _delayInterval = 10000;

    public OpenOrdersConsumer(ILogger<OpenOrdersConsumer> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        // _delayInterval = int.Parse(_configuration["Infrastructure:UserCreatedMessages:delayInterval"]!);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("[{Classe}] Worker's ativo", nameof(OpenOrdersConsumer));
            await Task.Delay(_delayInterval, stoppingToken);
        }

        await Task.CompletedTask;
    }
}
