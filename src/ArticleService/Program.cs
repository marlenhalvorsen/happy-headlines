using ArticleService.Data;
using ArticleService.Services;
using ArticleService.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddScoped<ArticleService.Services.ArticleService>();
builder.Services.AddScoped<ArticlePartitioner>();
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();