using RabbitMQ.Client;
using System.Text;

namespace RabbitMqExample;

public class MessagePublisher
{
    private readonly ConnectionFactory _factory;

    public MessagePublisher(
        string hostName = "localhost",
        int port = 5672,
        string userName = "guest",
        string password = "guest")
    {
        _factory = new ConnectionFactory()
        {
            HostName = hostName,
            Port = port,
            UserName = userName,
            Password = password
        };
    }

    public async Task PublishMessageAsync(
        string queueName,
        string message,
        CancellationToken cancellationToken = default)
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

        var body = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: queueName,
            body: body,
            cancellationToken: cancellationToken);

        Console.WriteLine($"Sent '{message}'");
    }
}
