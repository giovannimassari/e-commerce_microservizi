using Microsoft.AspNetCore.Mvc;
using Ordini.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Ordini.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class OrdiniController(IOrdineService ordineService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreaOrdine(
        [FromBody] CreaOrdineRequest request,
        CancellationToken cancellationToken)
    {
        Guid ordineId = await ordineService.CreaOrdineAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = ordineId }, new { id = ordineId });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var ordine = await ordineService.GetByIdAsync(id, cancellationToken);
        return ordine is null ? NotFound() : Ok(ordine);
    }
}