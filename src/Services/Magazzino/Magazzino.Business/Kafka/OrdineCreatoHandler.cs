using System.Text.Json;
using EventBus.Contracts.Events;
using Magazzino.Business.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Utility.Kafka.Abstractions.MessageHandlers;

namespace Magazzino.Business.Kafka;

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

        logger.LogInformation("Ricevuto OrderCreatedEvent per l'ordine {orderId}, riservo lo stock...", evento.OrderId);

        using var scope = scopeFactory.CreateScope();
        var stockRepository = scope.ServiceProvider.GetRequiredService<IStockRepository>();

        foreach (var item in evento.Items)
            await stockRepository.ReserveAsync(item.ProductId, item.Quantity, cancellationToken);
    }
}