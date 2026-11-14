using System.Text.Json;
using EventBus.Contracts.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pagamenti.Business.Services;
using Utility.Kafka.Abstractions.MessageHandlers;

namespace Pagamenti.Business.Kafka;

public class OrdineCreatoHandler(
    ILogger<OrdineCreatoHandler> logger,
    IServiceScopeFactory scopeFactory) : IMessageHandler<string, string>
{
    public async Task OnMessageReceivedAsync(string key, string message, CancellationToken cancellationToken = default)
    {
        var evento = JsonSerializer.Deserialize<OrderCreatedEvent>(message);

        if (evento is null)
        {
            logger.LogWarning("Impossibile deserializzare OrderCreatedEvent. Payload: {payload}", message);
            return;
        }

        logger.LogInformation("Ricevuto OrderCreatedEvent per l'ordine {orderId} (importo {importo}), elaboro il pagamento...",
            evento.OrderId, evento.TotalAmount);

        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IPagamentoService>();
        await service.ProcessaPagamentoAsync(evento, cancellationToken);
    }
}
