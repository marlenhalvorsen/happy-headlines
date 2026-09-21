using DraftService.Data;
using Microsoft.EntityFrameworkCore;
using DraftService.Repositories;
using DraftService.Services;
using Monitor;

var builder = WebApplication.CreateBuilder(args);

var monitor = new MonitorService();

builder.Services.AddControllers();

builder.Services.AddSingleton(monitor);
builder.Services.AddScoped<IDraftRepository, DraftRepository>();
builder.Services.AddScoped<IDraftService, DraftService.Services.DraftService>();

builder.Services.AddDbContext<DraftDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DraftDbContext>();
    dbContext.Database.Migrate();
}

app.MapControllers();

app.Run();