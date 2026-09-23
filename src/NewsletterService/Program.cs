using Monitor;
using NewsletterService.Messaging;
using NewsletterService.Telemetry;
using NewsletterService.Clients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton(
    new MonitorService(
        NewsletterTelemetry.ServiceName,
        NewsletterTelemetry.ActivitySource));

builder.Services.AddHostedService<RabbitMqArticleConsumer>();
builder.Services.AddHttpClient<ArticleClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ArticleService:BaseUrl"]
        ?? throw new InvalidOperationException(
            "ArticleService BaseUrl is not configured."));
});

var app = builder.Build();

// Resolve MonitorService so the tracing provider is created
// when NewsletterService starts.
var monitorService =
    app.Services.GetRequiredService<MonitorService>();

app.MapControllers();

app.Run();
