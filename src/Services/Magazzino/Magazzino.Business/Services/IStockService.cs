using Magazzino.Shared.DTOs;

namespace Magazzino.Business.Services;

public interface IStockService
{
    Task<List<StockDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<StockDto?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<StockDto> CreateOrIncrementAsync(CreaStockRequest request, CancellationToken cancellationToken = default);
    Task<StockDto> AggiornaQuantitaAsync(Guid productId, AggiornaStockRequest request, CancellationToken cancellationToken = default);
}