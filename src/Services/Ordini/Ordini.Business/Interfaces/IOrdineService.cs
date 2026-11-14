namespace Ordini.Business.Interfaces;

public interface IOrdineService
{
    Task<Guid> CreaOrdineAsync(CreaOrdineRequest request, CancellationToken cancellationToken = default);
    Task<OrdineDettaglioResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task GestisciPagamentoCompletatoAsync(Guid ordineId, CancellationToken cancellationToken = default);
    Task GestisciPagamentoFallitoAsync(Guid ordineId, string motivo, CancellationToken cancellationToken = default);
}

public record CreaOrdineRequest(Guid CustomerId, List<CreaOrdineItemRequest> Items);

public record CreaOrdineItemRequest(Guid ProductId, int Quantity, decimal UnitPrice);

public record OrdineDettaglioResponse(
    Guid Id,
    Guid CustomerId,
    decimal TotalAmount,
    string Stato,
    string? MotivoAnnullamento,
    DateTime CreatedAt,
    List<OrdineItemResponse> Items);

public record OrdineItemResponse(Guid ProductId, int Quantity, decimal UnitPrice);
