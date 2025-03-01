using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Shared.Abstractions;
using Domain.Shared.Abstractions;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Infrastructure.Messages;

public class MessageRepository : IMessageRepository
{
    private readonly string _connectionString = null!;

    private readonly IConfiguration _configuration;

    public MessageRepository(IConfiguration configuration)
    {
        var hostname = configuration["Infrastructure:RabbitMQ:Server"];
        var username = configuration["Infrastructure:RabbitMQ:Username"];
        var password = configuration["Infrastructure:RabbitMQ:Password"];
        var rabbitConnection = $"amqp://{username}:{password}@{hostname}/";

        _connectionString = rabbitConnection;
        _configuration = configuration;
    }

    public IMessageRepository CreateNewInstance()
    {
        return new MessageRepository(this._configuration);
    }

    public async Task PublishAsync<T>(
        T @event,
        string? exchangeName,
        string? exchangeType,
        string? routingKey,
        string queueName)
        where T : IDomainEvent
    {
        try
        {
            var factory = new ConnectionFactory() { Uri = new Uri(_connectionString) };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            if (!string.IsNullOrEmpty(exchangeName))
            {
                await channel.ExchangeDeclareAsync(exchangeName, exchangeType?.ToLower() ?? "direct", true);
            }

            await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            await channel.QueueBindAsync(queue: queueName,
                      exchange: exchangeName!,
                      routingKey: routingKey!);

            var options = new JsonSerializerOptions { WriteIndented = true };
            var message = JsonSerializer.Serialize<T>(@event, options);

            await channel.BasicPublishAsync<BasicProperties>(
                exchangeName!,
                routingKey ?? queueName,
                false, new BasicProperties(), Encoding.UTF8.GetBytes(message));

        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to publish message", ex);
        }
    }

    public async Task Consume<T>(IChannel sharedChannel, Action<T> action) where T : IDomainEvent
    {
        try
        {
            var consumer = new AsyncEventingBasicConsumer(sharedChannel);
            await sharedChannel.BasicConsumeAsync(queue: sharedChannel.CurrentQueue!, autoAck: false, consumer: consumer);

            consumer.ReceivedAsync += async (model, eventsArgs) =>
            {
                byte[] body = eventsArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var messageContent = JsonSerializer.Deserialize<T>(message);

                action(messageContent!);
                await sharedChannel.BasicAckAsync(deliveryTag: eventsArgs.DeliveryTag, multiple: false);
            };
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IChannel> StartNewChannel(string queueName)
    {
        var connection = await GetConnectionFactory();
        IChannel channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 100, global: false);
        return channel;
    }

    public async Task<IConnection> GetConnectionFactory()
    {
        var factory = new ConnectionFactory() { Uri = new Uri(_connectionString) };

        // connection that will recover automatically
        factory.AutomaticRecoveryEnabled = true;

        // attempt recovery every 10 seconds
        factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);

        var connection = await factory.CreateConnectionAsync();
        return connection;
    }
}
