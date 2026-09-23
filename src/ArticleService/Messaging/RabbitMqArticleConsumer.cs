using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ArticleService.Models;
using ArticleService.Telemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Context.Propagation;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ArticleService.Messaging;

public class RabbitMqArticleConsumer : BackgroundService
{
    private readonly ConnectionFactory _factory;
    private readonly IServiceScopeFactory _scopeFactory;

    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqArticleConsumer(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;

        _factory = new ConnectionFactory
        {
            HostName = "rabbitmq"
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _connection = await _factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        // Create the fanout exchange.
        await _channel.ExchangeDeclareAsync(
            exchange: "article-exchange",
            type: ExchangeType.Fanout,
            durable: true);

        // ArticleService has its own queue.
        await _channel.QueueDeclareAsync(
            queue: "article-service-queue",
            durable: true,
            exclusive: false,
            autoDelete: false);

        // Bind ArticleService's queue to the exchange.
        await _channel.QueueBindAsync(
            queue: "article-service-queue",
            exchange: "article-exchange",
            routingKey: "");

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            // Extract the trace context injected by PublisherService.
            var propagator = new TraceContextPropagator();

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

            // Continue the same distributed trace.
            using var activity =
                ArticleTelemetry.ActivitySource.StartActivity(
                    "Consume Article",
                    ActivityKind.Consumer,
                    propagationContext.ActivityContext);

            var body = eventArgs.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);

            var article = JsonSerializer.Deserialize<Article>(json);

            if (article == null)
                return;

            // BackgroundService is singleton, so create a scope
            // before resolving the scoped ArticleService.
            using var scope = _scopeFactory.CreateScope();

            var articleService =
                scope.ServiceProvider
                    .GetRequiredService<Services.ArticleService>();

            await articleService.CreateArticle(
                "global",
                article);
        };

        // Start consuming ArticleService's own queue.
        await _channel.BasicConsumeAsync(
            queue: "article-service-queue",
            autoAck: true,
            consumer: consumer);

        await Task.Delay(
            Timeout.Infinite,
            stoppingToken);
    }
}
