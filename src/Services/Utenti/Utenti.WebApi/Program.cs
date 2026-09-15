using Utenti.Repository;
using Microsoft.EntityFrameworkCore;

using Common.Auth;
using Utenti.Business.Interfaces;
using Utenti.Business.Services;
using Utenti.Repository.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<UtentiDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("UtentiDb")));

builder.Services.AddJwtTokenGenerator(builder.Configuration);
builder.Services.AddScoped<IUtenteRepository, UtenteRepository>();
builder.Services.AddScoped<IUtenteQueryService, UtenteQueryService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<PasswordHasher>();
builder.Services.AddControllers();

builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

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
