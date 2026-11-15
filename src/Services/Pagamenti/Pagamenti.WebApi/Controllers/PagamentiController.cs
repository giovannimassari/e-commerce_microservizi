using Common.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pagamenti.Business.Services;
using Pagamenti.Shared.DTOs;

namespace Pagamenti.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/pagamenti")]
public class PagamentiController(IPagamentoService service) : ControllerBase
{
    // GET api/pagamenti: elenco completo, solo Admin
    [HttpGet]
    [Authorize(Roles = Ruoli.Admin)]
    public async Task<ActionResult<List<PagamentoDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await service.GetAllAsync(cancellationToken));

    // GET api/pagamenti/ordine/{ordineId}: il proprietario dell'ordine o un Admin
    [HttpGet("ordine/{ordineId:guid}")]
    public async Task<ActionResult<PagamentoDto>> GetByOrdine(Guid ordineId, CancellationToken cancellationToken)
    {
        var pagamento = await service.GetByOrdineAsync(ordineId, cancellationToken);

        if (pagamento is null)
            return NotFound();

        if (!User.IsAdmin() && pagamento.CustomerId != User.GetUtenteId())
            return Forbid();

        return Ok(pagamento);
    }
}
