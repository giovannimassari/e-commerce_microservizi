using Microsoft.AspNetCore.Mvc;
using Utenti.Business.Interfaces;
using Utenti.Shared.DTOs;

namespace Utenti.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("registrazione")]
    public async Task<ActionResult<UtenteDto>> Registrazione(RegistrazioneRequest request)
    {
        try
        {
            var utente = await _authService.RegistraAsync(request);
            return CreatedAtAction(nameof(Registrazione), new { id = utente.Id }, utente);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { messaggio = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { messaggio = ex.Message });
        }
    }
}
