using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Shared.Abstractions;
using Domain.Shared.Abstractions;

using RabbitMQ.Client;

namespace Infrastructure.Messages;

public class MessageRepository : IMessageRepository
{
    private readonly string _connectionString = null!;

    public MessageRepository(string connectionString)
    {
        _connectionString = connectionString;
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
                channel.ExchangeDeclare(exchangeName, exchangeType?.ToLower());
            }
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var message = JsonSerializer.Serialize(@event);

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
}
