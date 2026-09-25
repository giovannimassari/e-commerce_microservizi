namespace Utenti.Shared.DTOs;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime Scadenza { get; set; }  // token's expire date
    public UtenteDto Utente { get; set; } = null!;
}
