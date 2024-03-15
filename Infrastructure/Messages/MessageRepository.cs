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

    public void PublishAsync<T>(
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
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            if (!string.IsNullOrEmpty(exchangeName))
            {
                channel.ExchangeDeclare(exchangeName, exchangeType?.ToLower(), true);
            }

            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind(queue: queueName,
                      exchange: exchangeName,
                      routingKey: routingKey);

            var options = new JsonSerializerOptions { WriteIndented = true };
            var message = JsonSerializer.Serialize<T>(@event, options);

            channel.BasicPublish(exchange: exchangeName,
                                routingKey: routingKey ?? queueName,
                                basicProperties: null,
                                body: Encoding.UTF8.GetBytes(message));

        }
        catch (Exception)
        {
            throw;
        }
    }

    public void Consume<T>(IModel sharedChannel, Action<T> action) where T : IDomainEvent
    {
        try
        {
            var consumer = new EventingBasicConsumer(sharedChannel);
            sharedChannel.BasicConsume(queue: sharedChannel.CurrentQueue, autoAck: false, consumer: consumer);
            consumer.Received += (model, eventsArgs) =>
            {
                byte[] body = eventsArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var messageContent = JsonSerializer.Deserialize<T>(message);

                action(messageContent!);
                sharedChannel.BasicAck(deliveryTag: eventsArgs.DeliveryTag, multiple: false);
            };
        }
        catch (Exception)
        {
            throw;
        }
    }

    public IModel StartNewChannel(string queueName)
    {
        var connection = GetConnectionFactory();
        IModel channel = connection.CreateModel();
        channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        channel.BasicQos(prefetchSize: 0, prefetchCount: 100, global: false);
        return channel;
    }

    public IConnection GetConnectionFactory()
    {
        var factory = new ConnectionFactory() { Uri = new Uri(_connectionString) };

        // connection that will recover automatically
        factory.AutomaticRecoveryEnabled = true;

        // attempt recovery every 10 seconds
        factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);

        var connection = factory.CreateConnection();
        return connection;
    }
}
