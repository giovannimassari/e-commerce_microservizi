namespace Magazzino.Business.Repositories;
using Data.Entities;

public interface IStockRepository
{
    Task ReserveAsync(Guid productId, int quantity, CancellationToken cancellationToken);
    Task<Stock?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
}