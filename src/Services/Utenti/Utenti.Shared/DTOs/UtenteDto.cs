using Utenti.Shared.Enums;

namespace Utenti.Shared.DTOs;

public class UtenteDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cognome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public RuoloUtente Ruolo { get; set; }
    public DateTime DataRegistrazione { get; set; }
}
