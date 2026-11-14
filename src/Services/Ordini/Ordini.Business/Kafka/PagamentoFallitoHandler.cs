using System.Text.Json;
using EventBus.Contracts.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ordini.Business.Interfaces;
using Utility.Kafka.Abstractions.MessageHandlers;

namespace Ordini.Business.Kafka;

public class PagamentoFallitoHandler(
    ILogger<PagamentoFallitoHandler> logger,
    IServiceScopeFactory scopeFactory) : IMessageHandler<string, string>
{
    public async Task OnMessageReceivedAsync(string key, string message, CancellationToken cancellationToken = default)
    {
        var evento = JsonSerializer.Deserialize<PagamentoFallitoEvent>(message);

        if (evento is null)
        {
            logger.LogWarning("Impossibile deserializzare PagamentoFallitoEvent. Payload: {payload}", message);
            return;
        }

        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IOrdineService>();
        await service.GestisciPagamentoFallitoAsync(evento.OrderId, evento.Motivo, cancellationToken);
    }
}
