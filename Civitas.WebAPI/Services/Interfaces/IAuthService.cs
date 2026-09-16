using Civitas.WebAPI.Objects.Dtos.Auth;

namespace Civitas.WebAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequest);
        Task<RegisterResponseDTO> Register(RegisterRequestDTO registerRequest);
        Task RequestPasswordRecovery(ForgotPasswordRequestDTO request, CancellationToken cancellationToken = default);
        Task ResetPassword(ResetPasswordRequestDTO request);
    }
}
