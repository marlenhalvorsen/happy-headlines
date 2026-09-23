using System.Diagnostics;
using System.Text;
using System.Text.Json;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using PublisherService.DTOs;
using PublisherService.Telemetry;
using RabbitMQ.Client;

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

        // Fanout exchange sends the article to every queue
        // that is bound to this exchange.
        await channel.ExchangeDeclareAsync(
            exchange: "article-exchange",
            type: ExchangeType.Fanout,
            durable: true);

        var json = JsonSerializer.Serialize(article);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = new BasicProperties
        {
            Headers = new Dictionary<string, object?>()
        };

        var propagator = Propagators.DefaultTextMapPropagator;

        // Inject the current trace context into the RabbitMQ message.
        propagator.Inject(
            new PropagationContext(
                activity?.Context ?? default,
                Baggage.Current),
            properties,
            (props, key, value) =>
            {
                props.Headers![key] =
                    Encoding.UTF8.GetBytes(value);
            });

        await channel.BasicPublishAsync(
            exchange: "article-exchange",
            routingKey: "",
            mandatory: false,
            basicProperties: properties,
            body: body);
    }
}
