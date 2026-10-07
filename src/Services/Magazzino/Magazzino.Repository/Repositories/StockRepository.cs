using Magazzino.Business.Repositories;
using Magazzino.Data.Context;
using Magazzino.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Magazzino.Repository.Repositories;

public class StockRepository : IStockRepository
{
    private readonly MagazzinoDbContext _context;

    public StockRepository(MagazzinoDbContext context)
    {
        _context = context;
    }

    public async Task<Stock?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.Stocks.FindAsync(new object[] { productId }, cancellationToken);
    }

    public async Task ReserveAsync(Guid productId, int quantityRequest, CancellationToken cancellationToken = default)
    {
        var stock = await _context.Stocks.FindAsync(new object[] { productId }, cancellationToken);

        if (stock is null)
        {
            throw new KeyNotFoundException($"Nessuno stock trovato per il prodotto {productId}.");
        }

        if (stock.QuantitaDisponibile < quantityRequest)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantityRequest),
                quantityRequest,
                $"Quantità richiesta ({quantityRequest}) superiore alla disponibilità ({stock.QuantitaDisponibile}).");
        }

        stock.QuantitaDisponibile -= quantityRequest;
        stock.UltimoAggiornamento = DateTime.UtcNow;

        // NOTA: in caso di accessi concorrenti sullo stesso prodotto, questo non basta
        // da solo a prevenire una race condition (vedi discussione precedente).
        // Per ora ci affidiamo alla gestione base di EF Core; da rivedere con
        // optimistic concurrency (RowVersion) o una transazione esplicita.
        await _context.SaveChangesAsync(cancellationToken);
    }
}