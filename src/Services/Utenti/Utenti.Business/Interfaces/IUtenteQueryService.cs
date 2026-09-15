using Utenti.Shared.DTOs;

namespace Utenti.Business.Interfaces;

public interface IUtenteQueryService
{
    Task<UtenteDto?> GetProfiloAsync(Guid utenteId);
}