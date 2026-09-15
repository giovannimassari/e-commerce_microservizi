using Microsoft.EntityFrameworkCore;
using Utenti.Repository.Entities;

namespace Utenti.Repository.Repositories;

public class UtenteRepository : IUtenteRepository
{
    private readonly UtentiDbContext _context;

    public UtenteRepository(UtentiDbContext context)
    {
        _context = context;
    }

    public async Task<Utente?> GetByEmailAsync(string email)
    {
        return await _context.Utenti.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Utente?> GetByIdAsync(Guid id)
    {
        return await _context.Utenti.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<bool> EsisteEmailAsync(string email)
    {
        return await _context.Utenti.AnyAsync(u => u.Email == email);
    }

    public async Task AggiungiAsync(Utente utente)
    {
        await _context.Utenti.AddAsync(utente);
    }

    public async Task SalvaModificheAsync()
    {
        await _context.SaveChangesAsync();
    }
}
