using System.Text.Json;
using Approvvigionamento.Business.Kafka;
using Approvvigionamento.Repository;
using Approvvigionamento.Repository.Entities;
using Approvvigionamento.Shared.DTOs;
using EventBus.Contracts.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Utility.Kafka.Abstractions.Clients;

namespace Approvvigionamento.Business.Services;

public class FornituraService(
    ApprovvigionamentoDbContext context,
    IProducerClient<string, string> producerClient,
    IOptions<ApprovvigionamentoProducerKafkaTopics> kafkaTopics,
    ILogger<FornituraService> logger) : IFornituraService
{
    public async Task RegistraRichiestaDaScortaAsync(ScortaBassaEvent evento, CancellationToken cancellationToken = default)
    {
        // Idempotenza: se per il prodotto c'è già una richiesta aperta non ne creo un'altra
        // (Kafka può riconsegnare lo stesso messaggio, e il Magazzino può segnalare più volte lo stesso prodotto).
        var esisteAperta = await context.Richieste
            .AnyAsync(r => r.ProductId == evento.ProductId && r.Stato == StatoRichiesta.Aperta, cancellationToken);

        if (esisteAperta)
        {
            logger.LogInformation("Richiesta già aperta per il prodotto {productId}, ignoro l'evento.", evento.ProductId);
            return;
        }

        var richiesta = new RichiestaRifornimento
        {
            ProductId = evento.ProductId,
            QuantitaDisponibileAllaRichiesta = evento.QuantitaDisponibile,
            // Si punta a riportare la disponibilità al doppio della soglia
            QuantitaRichiesta = Math.Max(evento.Soglia * 2 - evento.QuantitaDisponibile, 1)
        };

        context.Richieste.Add(richiesta);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Creata richiesta di rifornimento {id} per il prodotto {productId}: {qta} pezzi.",
            richiesta.Id, richiesta.ProductId, richiesta.QuantitaRichiesta);
    }

    public async Task<List<RichiestaRifornimentoDto>> GetRichiesteAsync(string? stato, CancellationToken cancellationToken = default)
    {
        var query = context.Richieste.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(stato) && Enum.TryParse<StatoRichiesta>(stato, true, out var statoParsed))
            query = query.Where(r => r.Stato == statoParsed);

        var richieste = await query.OrderByDescending(r => r.DataRichiesta).ToListAsync(cancellationToken);
        return richieste.Select(MappaRichiesta).ToList();
    }

    public async Task<RichiestaRifornimentoDto?> GetRichiestaAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var richiesta = await context.Richieste.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        return richiesta is null ? null : MappaRichiesta(richiesta);
    }

    public async Task<FornituraDto> CompletaRichiestaAsync(Guid richiestaId, CompletaRichiestaRequest request, CancellationToken cancellationToken = default)
    {
        if (request.QuantitaFornita <= 0)
            throw new ArgumentOutOfRangeException(nameof(request), "La quantità fornita deve essere maggiore di zero.");

        var richiesta = await context.Richieste.FirstOrDefaultAsync(r => r.Id == richiestaId, cancellationToken)
            ?? throw new KeyNotFoundException($"Richiesta di rifornimento {richiestaId} non trovata.");

        if (richiesta.Stato != StatoRichiesta.Aperta)
            throw new InvalidOperationException("La richiesta è già stata completata.");

        var fornitura = new Fornitura
        {
            RichiestaRifornimentoId = richiesta.Id,
            ProductId = richiesta.ProductId,
            FornitoreId = request.FornitoreId,
            QuantitaFornita = request.QuantitaFornita
        };

        richiesta.Stato = StatoRichiesta.Completata;
        richiesta.DataCompletamento = DateTime.UtcNow;

        context.Forniture.Add(fornitura);
        await context.SaveChangesAsync(cancellationToken);

        await PubblicaRiassortimentoCompletatoAsync(richiesta, fornitura, cancellationToken);

        return MappaFornitura(fornitura);
    }

    public async Task<List<FornituraDto>> GetFornitureAsync(CancellationToken cancellationToken = default)
    {
        var forniture = await context.Forniture.AsNoTracking()
            .OrderByDescending(f => f.DataFornitura)
            .ToListAsync(cancellationToken);
        return forniture.Select(MappaFornitura).ToList();
    }

    private async Task PubblicaRiassortimentoCompletatoAsync(RichiestaRifornimento richiesta, Fornitura fornitura, CancellationToken cancellationToken)
    {
        var evento = new RiassortimentoCompletatoEvent
        {
            RichiestaId = richiesta.Id,
            ProductId = fornitura.ProductId,
            QuantitaFornita = fornitura.QuantitaFornita,
            FornitoreId = fornitura.FornitoreId
        };

        await producerClient.ProduceAsync(
            topic: kafkaTopics.Value.RiassortimentoCompletato,
            key: richiesta.Id.ToString(),
            message: JsonSerializer.Serialize(evento),
            cancellationToken: cancellationToken);
    }

    private static RichiestaRifornimentoDto MappaRichiesta(RichiestaRifornimento r) => new()
    {
        Id = r.Id,
        ProductId = r.ProductId,
        QuantitaDisponibileAllaRichiesta = r.QuantitaDisponibileAllaRichiesta,
        QuantitaRichiesta = r.QuantitaRichiesta,
        Stato = r.Stato.ToString(),
        DataRichiesta = r.DataRichiesta,
        DataCompletamento = r.DataCompletamento
    };

    private static FornituraDto MappaFornitura(Fornitura f) => new()
    {
        Id = f.Id,
        RichiestaRifornimentoId = f.RichiestaRifornimentoId,
        ProductId = f.ProductId,
        FornitoreId = f.FornitoreId,
        QuantitaFornita = f.QuantitaFornita,
        DataFornitura = f.DataFornitura
    };
}
