using System.Text.Json;
using EventBus.Contracts.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pagamenti.Business.Kafka;
using Pagamenti.Repository;
using Pagamenti.Repository.Entities;
using Pagamenti.Shared.DTOs;
using Utility.Kafka.Abstractions.Clients;

namespace Pagamenti.Business.Services;

public class PagamentoService(
    PagamentiDbContext context,
    IProducerClient<string, string> producerClient,
    IOptions<PagamentiProducerKafkaTopics> kafkaTopics,
    ILogger<PagamentoService> logger) : IPagamentoService
{
    // Pagamento SIMULATO: non c'è nessun circuito reale. Un ordine viene rifiutato se l'importo
    // supera questo limite, così si può provare sia il percorso "completato" sia quello "fallito".
    private const decimal LimiteImporto = 5000m;

    public async Task ProcessaPagamentoAsync(OrderCreatedEvent ordine, CancellationToken cancellationToken = default)
    {
        // Idempotenza: se l'ordine ha già un pagamento (messaggio riconsegnato) non lo rielaboro
        if (await context.Pagamenti.AnyAsync(p => p.OrdineId == ordine.OrderId, cancellationToken))
        {
            logger.LogInformation("Pagamento già registrato per l'ordine {orderId}, ignoro l'evento.", ordine.OrderId);
            return;
        }

        var fallito = ordine.TotalAmount > LimiteImporto;

        var pagamento = new Pagamento
        {
            OrdineId = ordine.OrderId,
            CustomerId = ordine.CustomerId,
            Importo = ordine.TotalAmount,
            Stato = fallito ? StatoPagamento.Fallito : StatoPagamento.Completato,
            Motivo = fallito ? $"Importo superiore al limite consentito ({LimiteImporto})." : null
        };

        context.Pagamenti.Add(pagamento);
        await context.SaveChangesAsync(cancellationToken);

        // Il pagamento è salvato: un errore di pubblicazione non deve far rielaborare l'ordine.
        try
        {
            if (fallito)
                await PubblicaFallitoAsync(pagamento, cancellationToken);
            else
                await PubblicaCompletatoAsync(pagamento, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Pagamento {id} salvato ma evento non pubblicato.", pagamento.Id);
        }
    }

    public async Task<List<PagamentoDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var pagamenti = await context.Pagamenti.AsNoTracking()
            .OrderByDescending(p => p.DataPagamento)
            .ToListAsync(cancellationToken);
        return pagamenti.Select(Mappa).ToList();
    }

    public async Task<PagamentoDto?> GetByOrdineAsync(Guid ordineId, CancellationToken cancellationToken = default)
    {
        var pagamento = await context.Pagamenti.AsNoTracking()
            .FirstOrDefaultAsync(p => p.OrdineId == ordineId, cancellationToken);
        return pagamento is null ? null : Mappa(pagamento);
    }

    private Task PubblicaCompletatoAsync(Pagamento pagamento, CancellationToken cancellationToken)
    {
        var evento = new PagamentoCompletatoEvent
        {
            PagamentoId = pagamento.Id,
            OrderId = pagamento.OrdineId,
            CustomerId = pagamento.CustomerId,
            Importo = pagamento.Importo
        };

        return producerClient.ProduceAsync(
            topic: kafkaTopics.Value.PagamentoCompletato,
            key: pagamento.OrdineId.ToString(),
            message: JsonSerializer.Serialize(evento),
            cancellationToken: cancellationToken);
    }

    private Task PubblicaFallitoAsync(Pagamento pagamento, CancellationToken cancellationToken)
    {
        var evento = new PagamentoFallitoEvent
        {
            PagamentoId = pagamento.Id,
            OrderId = pagamento.OrdineId,
            CustomerId = pagamento.CustomerId,
            Importo = pagamento.Importo,
            Motivo = pagamento.Motivo ?? string.Empty
        };

        return producerClient.ProduceAsync(
            topic: kafkaTopics.Value.PagamentoFallito,
            key: pagamento.OrdineId.ToString(),
            message: JsonSerializer.Serialize(evento),
            cancellationToken: cancellationToken);
    }

    private static PagamentoDto Mappa(Pagamento p) => new()
    {
        Id = p.Id,
        OrdineId = p.OrdineId,
        CustomerId = p.CustomerId,
        Importo = p.Importo,
        Stato = p.Stato.ToString(),
        Motivo = p.Motivo,
        DataPagamento = p.DataPagamento
    };
}
