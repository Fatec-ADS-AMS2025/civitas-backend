using Civitas.WebAPI.Objects.Dtos.Auth;

namespace Civitas.WebAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequest);
    }
}
