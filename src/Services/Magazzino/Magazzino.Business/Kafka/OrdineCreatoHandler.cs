using System.Text.Json;
using EventBus.Contracts.Events;
using Magazzino.Business.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Utility.Kafka.Abstractions.Clients;
using Utility.Kafka.Abstractions.MessageHandlers;

namespace Magazzino.Business.Kafka;

public class OrdineCreatoHandler(
    ILogger<OrdineCreatoHandler> logger,
    IServiceScopeFactory scopeFactory,
    IProducerClient<string, string> producerClient,
    IOptions<MagazzinoProducerKafkaTopics> producerTopics) : IMessageHandler<string, string>
{
    // Sotto questa disponibilità il Magazzino segnala "scorta bassa" all'Approvvigionamento
    private const int SogliaScorta = 10;

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
        {
            await stockRepository.ReserveAsync(item.ProductId, item.Quantity, cancellationToken);

            var stock = await stockRepository.GetByProductIdAsync(item.ProductId, cancellationToken);
            if (stock is not null && stock.QuantitaDisponibile <= SogliaScorta)
                await PubblicaScortaBassaAsync(stock.ProductId, stock.QuantitaDisponibile, cancellationToken);
        }
    }

    private async Task PubblicaScortaBassaAsync(Guid productId, int quantitaDisponibile, CancellationToken cancellationToken)
    {
        // La riserva è già stata salvata: un errore di pubblicazione non deve far rielaborare l'ordine
        // (e scalare lo stock una seconda volta), quindi lo registro e proseguo.
        try
        {
            var evento = new ScortaBassaEvent
            {
                ProductId = productId,
                QuantitaDisponibile = quantitaDisponibile,
                Soglia = SogliaScorta
            };

            await producerClient.ProduceAsync(
                topic: producerTopics.Value.ScortaBassa,
                key: productId.ToString(),
                message: JsonSerializer.Serialize(evento),
                cancellationToken: cancellationToken);

            logger.LogInformation("Pubblicato ScortaBassaEvent per il prodotto {productId} (disponibili: {qta}).",
                productId, quantitaDisponibile);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Impossibile pubblicare ScortaBassaEvent per il prodotto {productId}.", productId);
        }
    }
}
