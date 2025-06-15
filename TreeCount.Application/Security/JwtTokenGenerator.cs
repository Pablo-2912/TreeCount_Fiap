using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TreeCount.Application.Interfaces;
using TreeCount.Repository.Identity;

namespace TreeCount.Application.Security
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _config;

        public JwtTokenGenerator(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(UserModel user)
        {
            // Lê as configurações do appsettings.json com validação
            var jwtKey = _config["Jwt:Key"] ?? throw new Exception("JWT Key not configured");
            var jwtIssuer = _config["Jwt:Issuer"] ?? throw new Exception("JWT Issuer not configured");
            var jwtAudience = _config["Jwt:Audience"] ?? throw new Exception("JWT Audience not configured");
            var expirationHoursStr = _config["Jwt:ExpireHours"];
            var expirationHours = string.IsNullOrEmpty(expirationHoursStr) ? 2 : int.Parse(expirationHoursStr);

            // Define os claims que irão no token
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim("name", user.Name),
    };

            // Adiciona roles se necessário futuramente
            // claims.Add(new Claim(ClaimTypes.Role, "Admin"));

            // Cria a chave e credenciais de assinatura
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Cria o token
            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expirationHours),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
