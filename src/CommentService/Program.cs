using CommentService.Data;
using Microsoft.EntityFrameworkCore;
using CommentService.Clients;
using CommentService.Services;
using Microsoft.Extensions.Http.Resilience;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<CommentDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("CommentDatabase")));
builder.Services.AddHttpClient<ProfanityClient>(client =>
    {
        client.BaseAddress = new Uri("http://localhost:5177");
    })
    .AddResilienceHandler("profanity-circuit-breaker", pipeline =>
    {
        pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
        {
            FailureRatio = 0.5,
            SamplingDuration = TimeSpan.FromSeconds(10),
            MinimumThroughput = 2,
            BreakDuration = TimeSpan.FromSeconds(15)
        });
    });
builder.Services.AddScoped<CommentManager>();
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

