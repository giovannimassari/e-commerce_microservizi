using Utenti.Shared.Enums;

namespace Utenti.Shared.DTOs;

public class RegistrazioneRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Cognome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public RuoloUtente Ruolo { get; set; } = RuoloUtente.Cliente;
}
