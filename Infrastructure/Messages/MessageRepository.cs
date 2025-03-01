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

<<<<<<< HEAD
    public void PublishAsync<T>(
=======
    public async Task PublishAsync<T>(
>>>>>>> 7b148c2b8ae7f452586616b92a970a41fd43347c
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
<<<<<<< HEAD
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
=======
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
>>>>>>> 7b148c2b8ae7f452586616b92a970a41fd43347c

            var options = new JsonSerializerOptions { WriteIndented = true };
            var message = JsonSerializer.Serialize<T>(@event, options);

<<<<<<< HEAD
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
=======
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
>>>>>>> 7b148c2b8ae7f452586616b92a970a41fd43347c
            {
                byte[] body = eventsArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var messageContent = JsonSerializer.Deserialize<T>(message);

                action(messageContent!);
<<<<<<< HEAD
                sharedChannel.BasicAck(deliveryTag: eventsArgs.DeliveryTag, multiple: false);
=======
                await sharedChannel.BasicAckAsync(deliveryTag: eventsArgs.DeliveryTag, multiple: false);
>>>>>>> 7b148c2b8ae7f452586616b92a970a41fd43347c
            };
        }
        catch (Exception)
        {
            throw;
        }
    }

<<<<<<< HEAD
    public IModel StartNewChannel(string queueName)
    {
        var connection = GetConnectionFactory();
        IModel channel = connection.CreateModel();
        channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        channel.BasicQos(prefetchSize: 0, prefetchCount: 100, global: false);
        return channel;
    }

    public IConnection GetConnectionFactory()
=======
    public async Task<IChannel> StartNewChannel(string queueName)
    {
        var connection = await GetConnectionFactory();
        IChannel channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 100, global: false);
        return channel;
    }

    public async Task<IConnection> GetConnectionFactory()
>>>>>>> 7b148c2b8ae7f452586616b92a970a41fd43347c
    {
        var factory = new ConnectionFactory() { Uri = new Uri(_connectionString) };

        // connection that will recover automatically
        factory.AutomaticRecoveryEnabled = true;

        // attempt recovery every 10 seconds
        factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);

<<<<<<< HEAD
        var connection = factory.CreateConnection();
=======
        var connection = await factory.CreateConnectionAsync();
>>>>>>> 7b148c2b8ae7f452586616b92a970a41fd43347c
        return connection;
    }
}
