using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Civitas.WebAPI.Objects.Dtos.Auth;
using Civitas.WebAPI.Objects.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Civitas.WebAPI.Services.Security
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtOptions _jwtOptions;

        public JwtTokenService(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        public LoginResponseDTO GenerateToken(Usuario usuario)
        {
            var expiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim("name", usuario.Nome),
                new Claim("tipousuario", usuario.TipoUsuario.ToString())
            };

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: expiresAtUtc,
                signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return new LoginResponseDTO
            {
                Token = token,
                ExpiresAtUtc = expiresAtUtc,
                Usuario = new LoginUsuarioDTO
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    TipoUsuario = usuario.TipoUsuario.ToString()
                }
            };
        }
    }
}
