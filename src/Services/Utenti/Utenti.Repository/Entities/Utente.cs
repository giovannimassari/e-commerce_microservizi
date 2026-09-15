using Utenti.Shared.Enums;

namespace Utenti.Repository.Entities;

public class Utente
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Nome { get; set; } = string.Empty;

    public string Cognome { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public RuoloUtente Ruolo { get; set; } = RuoloUtente.Cliente;

    public DateTime DataRegistrazione { get; set; } = DateTime.UtcNow;

    public bool Attivo { get; set; } = true;
}