using System.Security.Claims;

namespace Common.Auth;

public static class UserClaimsExtensions
{
    // Id dell'utente autenticato, letto dal claim "utente_id" del token. Null se assente o non valido.
    public static Guid? GetUtenteId(this ClaimsPrincipal user)
        => Guid.TryParse(user.FindFirst(ClaimTypesCustom.UtenteId)?.Value, out var id) ? id : null;

    public static bool IsAdmin(this ClaimsPrincipal user) => user.IsInRole(Ruoli.Admin);
}
