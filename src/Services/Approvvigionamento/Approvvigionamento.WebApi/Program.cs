using Common.Auth;
using Approvvigionamento.Business.Kafka;
using Approvvigionamento.Business.Services;
using Approvvigionamento.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Kafka: consumer (ScortaBassa) e producer (RiassortimentoCompletato)
builder.Services.AddScoped<ScortaBassaHandler>();
builder.Services.AddKafkaConsumerService<ApprovvigionamentoKafkaTopics, ApprovvigionamentoMessageHandlerFactory>(
    builder.Configuration);
builder.Services.AddKafkaProducerService<ApprovvigionamentoProducerKafkaTopics, ApprovvigionamentoProducerService>(
    builder.Configuration);

// Database
builder.Services.AddDbContext<ApprovvigionamentoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ApprovvigionamentoDb")));

// Service
builder.Services.AddScoped<IFornituraService, FornituraService>();

// Autenticazione JWT (stesso segreto/issuer/audience di Utenti)
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddControllers();

var app = builder.Build();

// Crea le tabelle se non esistono. Quando aggiungerai le migrations EF, sostituisci con Database.Migrate().
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<ApprovvigionamentoDbContext>().Database.EnsureCreated();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
