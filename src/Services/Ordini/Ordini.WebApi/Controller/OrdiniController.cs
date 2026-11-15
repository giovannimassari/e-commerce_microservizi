using Common.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordini.Business.Interfaces;

namespace Ordini.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class OrdiniController(IOrdineService ordineService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = Ruoli.Cliente + "," + Ruoli.Admin)]
    public async Task<IActionResult> CreaOrdine(
        [FromBody] CreaOrdineRequest request,
        CancellationToken cancellationToken)
    {
        var utenteId = User.GetUtenteId();
        if (utenteId is null)
            return Unauthorized();

        // Un cliente ordina solo per sé: il customerId viene dal token, non dal body.
        // Un Admin può invece creare un ordine per conto di un altro cliente.
        var richiesta = User.IsAdmin() ? request : request with { CustomerId = utenteId.Value };

        Guid ordineId = await ordineService.CreaOrdineAsync(richiesta, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = ordineId }, new { id = ordineId });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var ordine = await ordineService.GetByIdAsync(id, cancellationToken);

        if (ordine is null)
            return NotFound();

        // Si vedono solo i propri ordini, a meno di essere Admin
        if (!User.IsAdmin() && ordine.CustomerId != User.GetUtenteId())
            return Forbid();

        return Ok(ordine);
    }
}
