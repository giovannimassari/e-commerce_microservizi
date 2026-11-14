using Microsoft.EntityFrameworkCore;
using Pagamenti.Business.Kafka;
using Pagamenti.Business.Services;
using Pagamenti.Repository;

var builder = WebApplication.CreateBuilder(args);

// Kafka: consumer (OrdineCreato) e producer (PagamentoCompletato / PagamentoFallito)
builder.Services.AddScoped<OrdineCreatoHandler>();
builder.Services.AddKafkaConsumerService<PagamentiKafkaTopics, PagamentiMessageHandlerFactory>(
    builder.Configuration);
builder.Services.AddKafkaProducerService<PagamentiProducerKafkaTopics, PagamentiProducerService>(
    builder.Configuration);

// Database
builder.Services.AddDbContext<PagamentiDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PagamentiDb")));

// Service
builder.Services.AddScoped<IPagamentoService, PagamentoService>();

builder.Services.AddControllers();

var app = builder.Build();

// Crea le tabelle se non esistono. Quando aggiungerai le migrations EF, sostituisci con Database.Migrate().
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<PagamentiDbContext>().Database.EnsureCreated();
}

app.MapControllers();

app.Run();
