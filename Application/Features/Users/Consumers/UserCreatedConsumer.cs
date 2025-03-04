using System.Threading.Tasks;
using Application.Shared.Abstractions;
using Domain.Features.Users.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Application.Features.Users.Consumers;

public class UserCreatedConsumer : BackgroundService
{
    private readonly ILogger<UserCreatedConsumer> _logger;
    private readonly IConfiguration _configuration;
    private List<IMessageRepository> messageRepositories = [];
    private readonly IServiceProvider _serviceProvider;
    private readonly string _className = nameof(UserCreatedConsumer);

    private readonly int _instances = 0;
    private readonly int _delayInterval = 5000;
    private readonly string _queueName = null!;

    protected IConnection Connection { get; set; } = null!;
    protected IChannel SharedChannel { get; set; } = null!;

    private readonly string className = nameof(UserCreatedConsumer);

    public UserCreatedConsumer(
        ILogger<UserCreatedConsumer> logger,
        IConfiguration configuration,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        try
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;

            _queueName = _configuration["Infrastructure:UserCreatedMessages:queue"]!;
            _instances = int.Parse(_configuration["Infrastructure:UserCreatedMessages:consumerIntances"]!);
            _delayInterval = int.Parse(_configuration["Infrastructure:UserCreatedMessages:delayInterval"]!);

            Task.Run(async () => await ConfigureStart()).Wait();
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error starting User Created Consumer : Error: {ex}", className, ex);
            throw;
        }
    }

    private async Task ConfigureStart()
    {
        using (var scope = _serviceProvider.CreateScope())
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
                repo.ConsumeAsync<UserCreatedDomainEvent>(this.SharedChannel, this.ConsumeMessage);
            });

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("[{Classe}] ({intances}) Worker's ativo", nameof(UserCreatedConsumer), _instances);
                await Task.Delay(_delayInterval, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error when executing User Created Consumer : Error: {ex}", className, ex);
            throw;
        }
    }

    private async Task ConsumeMessage(UserCreatedDomainEvent userCreatedDomainEvent)
    {
        _logger.LogInformation("Consumindo criação de usuário: {Email}", userCreatedDomainEvent.Email);

        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedPaymentService = scope.ServiceProvider.GetRequiredService<IPaymentServices>();
            await scopedPaymentService.CreateUser(userCreatedDomainEvent.Id, null);
        }

    }
}
