using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MemoriaAfro.Core.Models;
using MemoriaAfro.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MemoriaAfro.Infra.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string Token, DateTime ExpiraEm) GerarToken(Usuario usuario, IList<string> roles)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var chave = jwtSettings["Key"]
            ?? throw new InvalidOperationException("Jwt:Key não configurada.");
        var minutosExpiracao = int.Parse(jwtSettings["ExpiresInMinutes"] ?? "120");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id),
            new(JwtRegisteredClaimNames.Email, usuario.Email),
            new(ClaimTypes.NameIdentifier, usuario.Id),
            new("nomeCompleto", usuario.NomeCompleto)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var credenciais = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chave)),
            SecurityAlgorithms.HmacSha256);

        var expiraEm = DateTime.UtcNow.AddMinutes(minutosExpiracao);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiraEm,
            signingCredentials: credenciais);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiraEm);
    }
}
