namespace Common.Auth;

public interface ITokenGenerator
{
    string GeneraToken(Guid utenteId, string email, string ruolo);
}
