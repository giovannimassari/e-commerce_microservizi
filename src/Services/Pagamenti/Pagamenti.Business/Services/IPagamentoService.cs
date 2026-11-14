using EventBus.Contracts.Events;
using Pagamenti.Shared.DTOs;

namespace Pagamenti.Business.Services;

public interface IPagamentoService
{
    Task ProcessaPagamentoAsync(OrderCreatedEvent ordine, CancellationToken cancellationToken = default);
    Task<List<PagamentoDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PagamentoDto?> GetByOrdineAsync(Guid ordineId, CancellationToken cancellationToken = default);
}
