using System.Diagnostics;
using System.Text;
using NewsletterService.Telemetry;
using OpenTelemetry.Context.Propagation;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NewsletterService.Messaging;

public class RabbitMqArticleConsumer : BackgroundService
{
    private readonly ConnectionFactory _factory;

    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqArticleConsumer()
    {
        _factory = new ConnectionFactory
        {
            HostName = "rabbitmq"
        };
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _connection = await _factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        //fanout exchange used by PublisherService.
        await _channel.ExchangeDeclareAsync(
            exchange: "article-exchange",
            type: ExchangeType.Fanout,
            durable: true);

        // NewsletterService has its own queue.
        await _channel.QueueDeclareAsync(
            queue: "newsletter-service-queue",
            durable: true,
            exclusive: false,
            autoDelete: false);

        // Every published article is copied to this queue.
        await _channel.QueueBindAsync(
            queue: "newsletter-service-queue",
            exchange: "article-exchange",
            routingKey: "");

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            var propagator = new TraceContextPropagator();

            // Extract the trace context PublisherService
            // placed in the RabbitMQ message.
            var propagationContext = propagator.Extract(
                default,
                eventArgs.BasicProperties,
                (properties, key) =>
                {
                    if (properties.Headers != null &&
                        properties.Headers.TryGetValue(key, out var value) &&
                        value is byte[] bytes)
                    {
                        return new[]
                        {
                            Encoding.UTF8.GetString(bytes)
                        };
                    }

                    return Array.Empty<string>();
                });

            // Continue PublisherService's trace.
            using var activity =
                NewsletterTelemetry.ActivitySource.StartActivity(
                    "Consume Article",
                    ActivityKind.Consumer,
                    propagationContext.ActivityContext);

            var body = eventArgs.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);

            Console.WriteLine(
                $"NewsletterService received article: {json}");

            await Task.CompletedTask;
        };

        await _channel.BasicConsumeAsync(
            queue: "newsletter-service-queue",
            autoAck: true,
            consumer: consumer);

        await Task.Delay(
            Timeout.Infinite,
            stoppingToken);
    }
}
