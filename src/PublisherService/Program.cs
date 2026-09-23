using PublisherService.Messaging;
using PublisherService.Services;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using Monitor;
using PublisherService.Telemetry;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddScoped<IPublishService, PublishService>();
builder.Services.AddScoped<IArticlePublisher, RabbitMqArticlePublisher>();
builder.Services.AddSingleton(
    new MonitorService(
        PublisherTelemetry.ServiceName,
        PublisherTelemetry.ActivitySource));

var app = builder.Build();

// Resolve MonitorService at startup to initialize OpenTelemetry tracing and the Zipkin exporter.
var monitorService = app.Services.GetRequiredService<MonitorService>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
app.UseHttpsRedirection();


app.Run();

