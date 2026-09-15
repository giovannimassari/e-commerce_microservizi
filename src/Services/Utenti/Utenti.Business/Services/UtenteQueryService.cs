using Utenti.Business.Interfaces;
using Utenti.Repository.Repositories;
using Utenti.Shared.DTOs;

namespace Utenti.Business.Services;

public class UtenteQueryService : IUtenteQueryService
{
    private readonly IUtenteRepository _utenteRepository;

    public UtenteQueryService(IUtenteRepository utenteRepository)
    {
        _utenteRepository = utenteRepository;
    }

    public async Task<UtenteDto?> GetProfiloAsync(Guid utenteId)
    {
        var utente = await _utenteRepository.GetByIdAsync(utenteId);

        if (utente is null)
        {
            return null;
        }

        return new UtenteDto
        {
            Id = utente.Id,
            Nome = utente.Nome,
            Cognome = utente.Cognome,
            Email = utente.Email,
            Ruolo = utente.Ruolo,
            DataRegistrazione = utente.DataRegistrazione
        };
    }
}