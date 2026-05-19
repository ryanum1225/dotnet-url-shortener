using dotnet_url_shortener.Services;
using dotnet_url_shortener.Data;
using Microsoft.EntityFrameworkCore;

// Load Environment Variables from .env file
EnvReader.Load(".env");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ShardAContext>(options =>
    options.UseSqlite("Data Source=" + Environment.GetEnvironmentVariable("SHARD_A")));
builder.Services.AddDbContext<ShardBContext>(options =>
    options.UseSqlite("Data Source=" + Environment.GetEnvironmentVariable("SHARD_B")));

builder.Services.AddMemoryCache();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<ShardService>();
builder.Services.AddScoped<UrlShorteningService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
