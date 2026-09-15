using System.Text.Json;
using EventBus.Contracts.Events;
using Microsoft.Extensions.Options;
using Ordini.Business.Domain;
using Ordini.Business.Interfaces;
using Ordini.Business.Kafka;
using Ordini.Business.Interfaces;
using Utility.Kafka.Abstractions.Clients;

namespace Ordini.Business.Services;

public class OrdineService(
    IOrdineRepository ordineRepository,
    IProducerClient<string, string> producerClient,
    IOptions<OrdiniKafkaTopics> kafkaTopics) : IOrdineService
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

        string payload = JsonSerializer.Serialize(evento);

        await producerClient.ProduceAsync(
            topic: kafkaTopics.Value.OrdineCreato,
            key: ordine.Id.ToString(),
            message: payload,
            cancellationToken: cancellationToken);
    }
}