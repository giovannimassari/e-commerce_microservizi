using Utenti.Shared.DTOs;

namespace Utenti.Business.Interfaces;

public interface IAuthService
{
    Task<UtenteDto> RegistraAsync(RegistrazioneRequest request);
    Task<LoginResponse> LoginAsync(LoginRequest request);
}
