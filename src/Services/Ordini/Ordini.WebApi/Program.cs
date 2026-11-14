using Common.Auth;
using Ordini.Business.Kafka;

using Ordini.Repository;
using Ordini.Business.Services;
using Ordini.Business.Interfaces;
using Microsoft.EntityFrameworkCore;
using Ordini.Repository.Repositories;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddKafkaProducerService<OrdiniKafkaTopics, OrdiniProducerService>(builder.Configuration);
builder.Services.AddScoped<PagamentoCompletatoHandler>();
builder.Services.AddScoped<PagamentoFallitoHandler>();
builder.Services.AddKafkaConsumerService<OrdiniConsumerKafkaTopics, OrdiniMessageHandlerFactory>(builder.Configuration);

builder.Services.AddDbContext<OrdiniDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("OrdiniDb")));

builder.Services.AddScoped<IOrdineRepository, OrdineRepository>();
builder.Services.AddScoped<IOrdineService, OrdineService>();

builder.Services.AddControllers();

builder.Services.AddJwtAuthentication(builder.Configuration);

// Add services to the container.

var app = builder.Build();

using (var scope = app.Services.CreateScope()) {
    scope.ServiceProvider.GetRequiredService<OrdiniDbContext>().Database.Migrate();
}

// Configure the HTTP request pipeline.

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

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

