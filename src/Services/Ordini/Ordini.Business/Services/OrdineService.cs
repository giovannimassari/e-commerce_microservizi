using System.Text.Json;
using EventBus.Contracts.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ordini.Business.Domain;
using Ordini.Business.Interfaces;
using Ordini.Business.Kafka;
using Utility.Kafka.Abstractions.Clients;

namespace Ordini.Business.Services;

public class OrdineService(
    IOrdineRepository ordineRepository,
    IProducerClient<string, string> producerClient,
    IOptions<OrdiniKafkaTopics> kafkaTopics,
    ILogger<OrdineService> logger) : IOrdineService
{
    public async Task<Guid> CreaOrdineAsync(CreaOrdineRequest request, CancellationToken cancellationToken = default)
    {
        var items = request.Items
            .Select(i => new OrdineItem(i.ProductId, i.Quantity, i.UnitPrice))
            .ToList();

        var ordine = new Ordine(request.CustomerId, items);

        await ordineRepository.AddAsync(ordine, cancellationToken);

        await PubblicaOrdineCreatoAsync(ordine, cancellationToken);

        return ordine.Id;
    }

    public async Task<OrdineDettaglioResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var ordine = await ordineRepository.GetByIdAsync(id, cancellationToken);

        if (ordine is null)
            return null;

        return new OrdineDettaglioResponse(
            ordine.Id,
            ordine.CustomerId,
            ordine.TotalAmount,
            ordine.Stato.ToString(),
            ordine.MotivoAnnullamento,
            ordine.CreatedAt,
            ordine.Items.Select(i => new OrdineItemResponse(i.ProductId, i.Quantity, i.UnitPrice)).ToList());
    }

    public async Task GestisciPagamentoCompletatoAsync(Guid ordineId, CancellationToken cancellationToken = default)
    {
        var ordine = await ordineRepository.GetByIdAsync(ordineId, cancellationToken);

        if (ordine is null)
        {
            logger.LogWarning("Pagamento completato per un ordine inesistente: {ordineId}", ordineId);
            return;
        }

        if (!ordine.SegnaComePagato())
        {
            logger.LogInformation("Ordine {ordineId} già in stato {stato}, ignoro PagamentoCompletato.", ordineId, ordine.Stato);
            return;
        }

        await ordineRepository.UpdateAsync(ordine, cancellationToken);
        logger.LogInformation("Ordine {ordineId} segnato come pagato.", ordineId);
    }

    public async Task GestisciPagamentoFallitoAsync(Guid ordineId, string motivo, CancellationToken cancellationToken = default)
    {
        var ordine = await ordineRepository.GetByIdAsync(ordineId, cancellationToken);

        if (ordine is null)
        {
            logger.LogWarning("Pagamento fallito per un ordine inesistente: {ordineId}", ordineId);
            return;
        }

        if (!ordine.Annulla(motivo))
        {
            logger.LogInformation("Ordine {ordineId} già in stato {stato}, ignoro PagamentoFallito.", ordineId, ordine.Stato);
            return;
        }

        await ordineRepository.UpdateAsync(ordine, cancellationToken);
        logger.LogInformation("Ordine {ordineId} annullato: {motivo}", ordineId, motivo);

        // L'annullamento è salvato: un errore di pubblicazione non deve far rielaborare il messaggio.
        try
        {
            await PubblicaOrdineAnnullatoAsync(ordine, motivo, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ordine {ordineId} annullato ma OrdineAnnullatoEvent non pubblicato: lo stock non è stato rilasciato.", ordineId);
        }
    }

    private async Task PubblicaOrdineCreatoAsync(Ordine ordine, CancellationToken cancellationToken)
    {
        var evento = new OrderCreatedEvent
        {
            OrderId = ordine.Id,
            CustomerId = ordine.CustomerId,
            TotalAmount = ordine.TotalAmount,
            Items = ordine.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        await producerClient.ProduceAsync(
            topic: kafkaTopics.Value.OrdineCreato,
            key: ordine.Id.ToString(),
            message: JsonSerializer.Serialize(evento),
            cancellationToken: cancellationToken);
    }

    private async Task PubblicaOrdineAnnullatoAsync(Ordine ordine, string motivo, CancellationToken cancellationToken)
    {
        var evento = new OrdineAnnullatoEvent
        {
            OrderId = ordine.Id,
            CustomerId = ordine.CustomerId,
            Motivo = motivo,
            Items = ordine.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        await producerClient.ProduceAsync(
            topic: kafkaTopics.Value.OrdineAnnullato,
            key: ordine.Id.ToString(),
            message: JsonSerializer.Serialize(evento),
            cancellationToken: cancellationToken);
    }
}
