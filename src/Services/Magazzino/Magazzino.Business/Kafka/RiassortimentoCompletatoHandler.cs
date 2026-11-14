using System.Text.Json;
using EventBus.Contracts.Events;
using Magazzino.Business.Repositories;
using Magazzino.Data.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Utility.Kafka.Abstractions.MessageHandlers;

namespace Magazzino.Business.Kafka;

public class RiassortimentoCompletatoHandler(
    ILogger<RiassortimentoCompletatoHandler> logger,
    IServiceScopeFactory scopeFactory) : IMessageHandler<string, string>
{
    public async Task OnMessageReceivedAsync(string key, string message, CancellationToken cancellationToken = default)
    {
        var evento = JsonSerializer.Deserialize<RiassortimentoCompletatoEvent>(message);

        if (evento is null)
        {
            logger.LogWarning("Impossibile deserializzare RiassortimentoCompletatoEvent. Payload: {payload}", message);
            return;
        }

        logger.LogInformation("Riassortimento completato per il prodotto {productId}: +{qta} pezzi.",
            evento.ProductId, evento.QuantitaFornita);

        using var scope = scopeFactory.CreateScope();
        var stockRepository = scope.ServiceProvider.GetRequiredService<IStockRepository>();

        var stock = await stockRepository.GetByProductIdAsync(evento.ProductId, cancellationToken);

        if (stock is null)
        {
            await stockRepository.AddAsync(new Stock
            {
                ProductId = evento.ProductId,
                QuantitaDisponibile = evento.QuantitaFornita,
                UltimoAggiornamento = DateTime.UtcNow
            }, cancellationToken);
            return;
        }

        stock.QuantitaDisponibile += evento.QuantitaFornita;
        stock.UltimoAggiornamento = DateTime.UtcNow;
        await stockRepository.UpdateAsync(stock, cancellationToken);
    }
}
