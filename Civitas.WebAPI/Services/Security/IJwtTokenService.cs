using Civitas.WebAPI.Objects.Dtos.Auth;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Services.Security
{
    public interface IJwtTokenService
    {
        LoginResponseDTO GenerateToken(Usuario usuario);
    }
}
