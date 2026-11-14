using Microsoft.AspNetCore.Mvc;
using Pagamenti.Business.Services;
using Pagamenti.Shared.DTOs;

namespace Pagamenti.WebApi.Controllers;

[ApiController]
[Route("api/pagamenti")]
public class PagamentiController(IPagamentoService service) : ControllerBase
{
    // GET api/pagamenti
    [HttpGet]
    public async Task<ActionResult<List<PagamentoDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await service.GetAllAsync(cancellationToken));

    // GET api/pagamenti/ordine/{ordineId}
    [HttpGet("ordine/{ordineId:guid}")]
    public async Task<ActionResult<PagamentoDto>> GetByOrdine(Guid ordineId, CancellationToken cancellationToken)
    {
        var pagamento = await service.GetByOrdineAsync(ordineId, cancellationToken);
        return pagamento is null ? NotFound() : Ok(pagamento);
    }
}
