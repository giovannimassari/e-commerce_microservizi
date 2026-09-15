using System.Security.Claims;
using Common.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utenti.Business.Interfaces;

namespace Utenti.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfiloController : ControllerBase
{
    private readonly IUtenteQueryService _utenteQueryService;

    public ProfiloController(IUtenteQueryService utenteQueryService)
    {
        _utenteQueryService = utenteQueryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfilo()
    {
        var utenteIdClaim = User.FindFirstValue(ClaimTypesCustom.UtenteId);

        if (utenteIdClaim is null || !Guid.TryParse(utenteIdClaim, out var utenteId))
        {
            return Unauthorized();
        }

        var utente = await _utenteQueryService.GetProfiloAsync(utenteId);

        if (utente is null)
        {
            return NotFound();
        }

        return Ok(utente);
    }
}