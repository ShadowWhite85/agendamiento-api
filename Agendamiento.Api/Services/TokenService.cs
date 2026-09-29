using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Agendamiento.Api.Dtos;
using Agendamiento.Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace Agendamiento.Api.Services;

public class TokenService(IConfiguration configuration)
{
    public TokenResponse Crear(Usuario usuario)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var minutos = int.Parse(configuration["Jwt:ExpirationMinutes"] ?? "60");
        var expira = DateTime.UtcNow.AddMinutes(minutos);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(JwtRegisteredClaimNames.Name, usuario.Nombre),
            new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
        };

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: expira,
            signingCredentials: credentials);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return new TokenResponse(jwt, usuario.Nombre, usuario.Email, usuario.Rol.ToString(), expira);
    }
}
