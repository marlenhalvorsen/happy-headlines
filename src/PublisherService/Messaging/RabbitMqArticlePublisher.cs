using System.Text;
using System.Text.Json;
using PublisherService.DTOs;
using RabbitMQ.Client;
using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using PublisherService.Telemetry;

namespace PublisherService.Messaging;

public class RabbitMqArticlePublisher : IArticlePublisher
{
    private readonly ConnectionFactory _factory;

    public RabbitMqArticlePublisher()
    {
        _factory = new ConnectionFactory
        {
            HostName = "rabbitmq"
        };
    }

    public async Task PublishAsync(PublishArticleDto article)
    {

        using var activity = PublisherTelemetry.ActivitySource.StartActivity(
            "Publish Article",
            ActivityKind.Producer);

        await using var connection =
            await _factory.CreateConnectionAsync();

        await using var channel =
            await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: "article-queue",
            durable: true,
            exclusive: false,
            autoDelete: false);

        var json = JsonSerializer.Serialize(article);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = new BasicProperties
        {
            Headers = new Dictionary<string, object?>()
        };

        var propagator = Propagators.DefaultTextMapPropagator;

        propagator.Inject(
            new PropagationContext(
                activity?.Context ?? default,
                Baggage.Current),
            properties,
            (props, key, value) =>
            {
                props.Headers![key] = Encoding.UTF8.GetBytes(value);
            });

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: "article-queue",
            mandatory: false,
            basicProperties: properties,
            body: body); 
    }
}
