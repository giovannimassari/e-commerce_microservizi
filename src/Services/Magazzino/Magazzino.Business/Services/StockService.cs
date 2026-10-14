using Magazzino.Business.Repositories;
using Magazzino.Data.Entities;
using Magazzino.Shared.DTOs;

namespace Magazzino.Business.Services;

public class StockService(IStockRepository stockRepository) : IStockService
{
    public async Task<List<StockDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var stocks = await stockRepository.GetAllAsync(cancellationToken);
        return stocks.Select(ToDto).ToList();
    }

    public async Task<StockDto?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var stock = await stockRepository.GetByProductIdAsync(productId, cancellationToken);
        return stock is null ? null : ToDto(stock);
    }

    public async Task<StockDto> CreateOrIncrementAsync(CreaStockRequest request, CancellationToken cancellationToken = default)
    {
        if (request.QuantitaIniziale < 0)
            throw new ArgumentOutOfRangeException(
                nameof(request),
                request.QuantitaIniziale,
                "La quantità iniziale non può essere negativa.");

        var existing = await stockRepository.GetByProductIdAsync(request.ProductId, cancellationToken);

        if (existing is null)
        {
            var nuovo = new Stock
            {
                ProductId = request.ProductId,
                QuantitaDisponibile = request.QuantitaIniziale,
                QuantitaRiservata = 0,
                UltimoAggiornamento = DateTime.UtcNow
            };

            await stockRepository.AddAsync(nuovo, cancellationToken);
            return ToDto(nuovo);
        }

        existing.QuantitaDisponibile += request.QuantitaIniziale;
        existing.UltimoAggiornamento = DateTime.UtcNow;

        await stockRepository.UpdateAsync(existing, cancellationToken);
        return ToDto(existing);
    }

    public async Task<StockDto> AggiornaQuantitaAsync(Guid productId, AggiornaStockRequest request, CancellationToken cancellationToken = default)
    {
        var stock = await stockRepository.GetByProductIdAsync(productId, cancellationToken)
            ?? throw new KeyNotFoundException($"Nessuno stock trovato per il prodotto {productId}.");

        int nuovaQuantita = stock.QuantitaDisponibile + request.Delta;

        if (nuovaQuantita < 0)
            throw new ArgumentOutOfRangeException(
                nameof(request),
                request.Delta,
                $"Il delta ({request.Delta}) porterebbe la quantità a {nuovaQuantita}, che è negativa.");

        stock.QuantitaDisponibile = nuovaQuantita;
        stock.UltimoAggiornamento = DateTime.UtcNow;

        await stockRepository.UpdateAsync(stock, cancellationToken);
        return ToDto(stock);
    }

    private static StockDto ToDto(Stock stock) => new()
    {
        ProductId = stock.ProductId,
        QuantitaDisponibile = stock.QuantitaDisponibile,
        QuantitaRiservata = stock.QuantitaRiservata,
        UltimoAggiornamento = stock.UltimoAggiornamento
    };
}