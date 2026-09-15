namespace Utenti.Shared.DTOs;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime Scadenza { get; set; }
    public UtenteDto Utente { get; set; } = null!;
}
