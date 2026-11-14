using System.Text.Json;
using EventBus.Contracts.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ordini.Business.Interfaces;
using Utility.Kafka.Abstractions.MessageHandlers;

namespace Ordini.Business.Kafka;

public class PagamentoCompletatoHandler(
    ILogger<PagamentoCompletatoHandler> logger,
    IServiceScopeFactory scopeFactory) : IMessageHandler<string, string>
{
    public async Task OnMessageReceivedAsync(string key, string message, CancellationToken cancellationToken = default)
    {
        var evento = JsonSerializer.Deserialize<PagamentoCompletatoEvent>(message);

        if (evento is null)
        {
            logger.LogWarning("Impossibile deserializzare PagamentoCompletatoEvent. Payload: {payload}", message);
            return;
        }

        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IOrdineService>();
        await service.GestisciPagamentoCompletatoAsync(evento.OrderId, cancellationToken);
    }
}
