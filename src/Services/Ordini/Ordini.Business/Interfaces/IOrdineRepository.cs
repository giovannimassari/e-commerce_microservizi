using Ordini.Business.Domain;

namespace Ordini.Business.Interfaces;

public interface IOrdineRepository
{
    Task AddAsync(Ordine ordine, CancellationToken cancellationToken = default);
    Task<Ordine?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}