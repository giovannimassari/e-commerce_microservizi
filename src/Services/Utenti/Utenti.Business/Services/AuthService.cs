using Common.Auth;
using Utenti.Business.Interfaces;
using Utenti.Repository.Entities;
using Utenti.Repository.Repositories;
using Utenti.Shared.DTOs;

namespace Utenti.Business.Services;

public class AuthService : IAuthService
{
    private readonly IUtenteRepository _utenteRepository;
    private readonly PasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;

    public AuthService(IUtenteRepository utenteRepository, PasswordHasher passwordHasher, ITokenGenerator tokenGenerator)
    {
        _utenteRepository = utenteRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<UtenteDto> RegistraAsync(RegistrazioneRequest request)
    {
        if (await _utenteRepository.EsisteEmailAsync(request.Email))
        {
            throw new InvalidOperationException("Email gia' registrata.");
        }

        var utente = new Utente
        {
            Nome = request.Nome,
            Cognome = request.Cognome,
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            Ruolo = request.Ruolo
        };

        await _utenteRepository.AggiungiAsync(utente);
        await _utenteRepository.SalvaModificheAsync();

        return MappaDto(utente);
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var utente = await _utenteRepository.GetByEmailAsync(request.Email);
        
        if(utente is null)
            throw new UnauthorizedAccessException("Non esiste nessun utente con questa email.");
        
        if(!utente.Attivo)
            throw new UnauthorizedAccessException("Questo utente non è più attivo.");

        if (!_passwordHasher.VerifyPassword(request.Password, utente.PasswordHash))
            throw new UnauthorizedAccessException("Password errata.");

        var token = _tokenGenerator.GeneraToken(utente.Id, utente.Email, utente.Ruolo.ToString());

        return new LoginResponse
        {
            Token = token,
            Scadenza = DateTime.UtcNow.AddMinutes(60),
            Utente = MappaDto(utente)
        };
    }

    private static UtenteDto MappaDto(Utente utente) => new()
    {
        Id = utente.Id,
        Nome = utente.Nome,
        Cognome = utente.Cognome,
        Email = utente.Email,
        Ruolo = utente.Ruolo,
        DataRegistrazione = utente.DataRegistrazione
    };
}
