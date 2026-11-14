using Approvvigionamento.Business.Services;
using Approvvigionamento.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Approvvigionamento.WebApi.Controllers;

[ApiController]
[Route("api/forniture")]
public class FornitureController(IFornituraService service) : ControllerBase
{
    // GET api/forniture/richieste?stato=Aperta
    [HttpGet("richieste")]
    public async Task<ActionResult<List<RichiestaRifornimentoDto>>> GetRichieste(
        [FromQuery] string? stato, CancellationToken cancellationToken)
        => Ok(await service.GetRichiesteAsync(stato, cancellationToken));

    // GET api/forniture/richieste/{id}
    [HttpGet("richieste/{id:guid}")]
    public async Task<ActionResult<RichiestaRifornimentoDto>> GetRichiesta(Guid id, CancellationToken cancellationToken)
    {
        var richiesta = await service.GetRichiestaAsync(id, cancellationToken);
        return richiesta is null ? NotFound() : Ok(richiesta);
    }

    // POST api/forniture/richieste/{id}/completa
    [HttpPost("richieste/{id:guid}/completa")]
    public async Task<ActionResult<FornituraDto>> Completa(
        Guid id, CompletaRichiestaRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await service.CompletaRichiestaAsync(id, request, cancellationToken));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { messaggio = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { messaggio = ex.Message });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { messaggio = ex.Message });
        }
    }

    // GET api/forniture
    [HttpGet]
    public async Task<ActionResult<List<FornituraDto>>> GetForniture(CancellationToken cancellationToken)
        => Ok(await service.GetFornitureAsync(cancellationToken));
}
