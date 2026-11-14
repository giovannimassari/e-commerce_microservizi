using Common.Auth;
using Magazzino.Business.Kafka;
using Magazzino.Business.Repositories;
using Magazzino.Business.Services;
using Magazzino.Data.Context;
using Magazzino.Repository.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Kafka
builder.Services.AddScoped<OrdineCreatoHandler>();
builder.Services.AddKafkaConsumerService<MagazzinoKafkaTopics, MagazzinoMessageHandlerFactory>(builder.Configuration);
builder.Services.AddScoped<RiassortimentoCompletatoHandler>();
builder.Services.AddKafkaProducerService<MagazzinoProducerKafkaTopics, MagazzinoProducerService>(builder.Configuration);

// Database
builder.Services.AddDbContext<MagazzinoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MagazzinoDb")));

// Repository e Service
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IStockService, StockService>();

// Controllers
builder.Services.AddControllers();

builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope()) {
    scope.ServiceProvider.GetRequiredService<MagazzinoDbContext>().Database.Migrate();
}

app.UseHttpsRedirection();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
