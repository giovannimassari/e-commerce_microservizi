namespace Ordini.Business.Interfaces;

public interface IOrdineService
{
    Task<Guid> CreaOrdineAsync(CreaOrdineRequest request, CancellationToken cancellationToken = default);
}

public record CreaOrdineRequest(Guid CustomerId, List<CreaOrdineItemRequest> Items);

public record CreaOrdineItemRequest(Guid ProductId, int Quantity, decimal UnitPrice);