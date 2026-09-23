using ArticleService.Data;
using ArticleService.Services;
using ArticleService.Repositories;
using Microsoft.EntityFrameworkCore;
using ArticleService.Messaging;
using ArticleService.Telemetry;
using Monitor;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddScoped<ArticleService.Services.ArticleService>();
builder.Services.AddScoped<ArticlePartitioner>();
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddSingleton(
    new MonitorService(
        ArticleTelemetry.ServiceName,
        ArticleTelemetry.ActivitySource)); 
builder.Services.AddHostedService<RabbitMqArticleConsumer>();

var app = builder.Build();

// Initialize OpenTelemetry tracing and the Zipkin exporter.
var monitorService = app.Services.GetRequiredService<MonitorService>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();