using System.Security.Claims;
using Civitas.WebAPI.Objects.Dtos.Auth;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Services.Security
{
    public interface IJwtTokenService
    {
        /// <summary>
        /// Gera um token JWT para o usuário autenticado.
        /// </summary>
        /// <param name="usuario">Usuário para o qual o token será gerado.</param>
        /// <param name="extraClaims">Claims adicionais opcionais a incluir no token.</param>
        LoginResponseDTO GenerateToken(Usuario usuario, IEnumerable<Claim>? extraClaims = null);
    }
}
