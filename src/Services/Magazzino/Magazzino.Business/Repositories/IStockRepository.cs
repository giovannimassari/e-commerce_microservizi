using Magazzino.Data.Entities;

namespace Magazzino.Business.Repositories;

public interface IStockRepository
{
    Task ReserveAsync(Guid productId, int quantity, CancellationToken cancellationToken);
    Task<Stock?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<List<Stock>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Stock stock, CancellationToken cancellationToken = default);
    Task UpdateAsync(Stock stock, CancellationToken cancellationToken = default);
}