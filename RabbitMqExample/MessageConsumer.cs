using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMqExample;

public class MessageConsumer
{
    private readonly ConnectionFactory _factory;

    public MessageConsumer(string hostName = "localhost", int port = 5672, string userName = "guest", string password = "guest")
    {
        _factory = new ConnectionFactory()
        {
            HostName = hostName,
            Port = port,
            UserName = userName,
            Password = password
        };
    }

    public async Task ConsumeMessagesAsync(string queueName, CancellationToken cancellationToken = default)
    {
        await using var connection = await _factory.CreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Received '{message}'");

            await channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken);
        };

        await channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken);

        Console.WriteLine($"Waiting for messages on queue '{queueName}'. Press enter to stop.");
        Console.ReadLine();
    }
}

