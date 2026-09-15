using Microsoft.AspNetCore.Mvc;
using Ordini.Business.Interfaces;

namespace Ordini.WebApi.Controllers;

[ApiController]
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
    public IActionResult GetById(Guid id)
    {
        // Placeholder per ora — lo implementiamo se serve leggere l'ordine
        return Ok(new { id });
    }
}