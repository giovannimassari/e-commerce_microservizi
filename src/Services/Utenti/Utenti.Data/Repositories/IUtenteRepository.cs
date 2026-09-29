using Utenti.Repository.Entities;

namespace Utenti.Repository.Repositories;

public interface IUtenteRepository
{
    Task<Utente?> GetByEmailAsync(string email);
    Task<Utente?> GetByIdAsync(Guid id);
    Task<bool> EsisteEmailAsync(string email);
    Task AggiungiAsync(Utente utente);
    Task SalvaModificheAsync();
}
