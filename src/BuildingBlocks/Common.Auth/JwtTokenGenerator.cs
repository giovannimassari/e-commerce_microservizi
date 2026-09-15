using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Common.Auth;

public class JwtTokenGenerator : ITokenGenerator
{
    private readonly JwtSettings _settings;

    public JwtTokenGenerator(IOptions<JwtSettings> options)
    {
        _settings = options.Value;
    }

    public string GeneraToken(Guid utenteId, string email, string ruolo)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, utenteId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(ClaimTypesCustom.UtenteId, utenteId.ToString()),
            new(ClaimTypesCustom.Ruolo, ruolo),
            new(ClaimTypes.Role, ruolo)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.ExpirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
