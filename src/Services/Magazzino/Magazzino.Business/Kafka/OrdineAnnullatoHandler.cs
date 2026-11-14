using System.Text.Json;
using EventBus.Contracts.Events;
using Magazzino.Business.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Utility.Kafka.Abstractions.MessageHandlers;

namespace Magazzino.Business.Kafka;

public class OrdineAnnullatoHandler(
    ILogger<OrdineAnnullatoHandler> logger,
    IServiceScopeFactory scopeFactory) : IMessageHandler<string, string>
{
    public async Task OnMessageReceivedAsync(string key, string message, CancellationToken cancellationToken = default)
    {
        var evento = JsonSerializer.Deserialize<OrdineAnnullatoEvent>(message);

        if (evento is null)
        {
            logger.LogWarning("Impossibile deserializzare OrdineAnnullatoEvent. Payload: {payload}", message);
            return;
        }

        logger.LogInformation("Ordine {orderId} annullato ({motivo}): rilascio lo stock prenotato.",
            evento.OrderId, evento.Motivo);

        using var scope = scopeFactory.CreateScope();
        var stockRepository = scope.ServiceProvider.GetRequiredService<IStockRepository>();

        foreach (var item in evento.Items)
        {
            var stock = await stockRepository.GetByProductIdAsync(item.ProductId, cancellationToken);

            if (stock is null)
            {
                logger.LogWarning("Stock non trovato per il prodotto {productId}, impossibile rilasciare.", item.ProductId);
                continue;
            }

            stock.QuantitaDisponibile += item.Quantity;
            // Se la riserva è tracciata la scalo; se non lo è (valore 0) resta a 0, mai negativa.
            stock.QuantitaRiservata = Math.Max(0, stock.QuantitaRiservata - item.Quantity);
            stock.UltimoAggiornamento = DateTime.UtcNow;

            await stockRepository.UpdateAsync(stock, cancellationToken);
        }
    }
}
