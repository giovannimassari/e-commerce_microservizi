using Magazzino.Business.Kafka;
using Magazzino.Business.Repositories;
using Magazzino.Repository.Repositories;
using Magazzino.Data.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<OrdineCreatoHandler>();

builder.Services.AddKafkaConsumerService<MagazzinoKafkaTopics, MagazzinoMessageHandlerFactory>(
    builder.Configuration);

builder.Services.AddDbContext<MagazzinoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MagazzinoDb")));

builder.Services.AddScoped<IStockRepository, StockRepository>();

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
