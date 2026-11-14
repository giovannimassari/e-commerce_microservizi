using System.Text.Json;
using Approvvigionamento.Business.Services;
using EventBus.Contracts.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Utility.Kafka.Abstractions.MessageHandlers;

namespace Approvvigionamento.Business.Kafka;

public class ScortaBassaHandler(
    ILogger<ScortaBassaHandler> logger,
    IServiceScopeFactory scopeFactory) : IMessageHandler<string, string>
{
    public async Task OnMessageReceivedAsync(string key, string message, CancellationToken cancellationToken = default)
    {
        var evento = JsonSerializer.Deserialize<ScortaBassaEvent>(message);

        if (evento is null)
        {
            logger.LogWarning("Impossibile deserializzare ScortaBassaEvent. Payload: {payload}", message);
            return;
        }

        logger.LogInformation(
            "Ricevuto ScortaBassaEvent per il prodotto {productId} (disponibili: {qta}, soglia: {soglia})",
            evento.ProductId, evento.QuantitaDisponibile, evento.Soglia);

        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IFornituraService>();
        await service.RegistraRichiestaDaScortaAsync(evento, cancellationToken);
    }
}
