using Microsoft.EntityFrameworkCore;
using Ordini.Business.Domain;
using Ordini.Business.Interfaces;
using Ordini.Repository;

namespace Ordini.Repository.Repositories;

public class OrdineRepository(OrdiniDbContext context) : IOrdineRepository
{
    public async Task AddAsync(Ordine ordine, CancellationToken cancellationToken = default)
    {
        await context.Ordini.AddAsync(ordine, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Ordine?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Ordini
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
}