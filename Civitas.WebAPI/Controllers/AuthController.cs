using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Auth;
using Civitas.WebAPI.Services.Interfaces;
using Civitas.WebAPI.Services.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Controllers
{
    [Authorize]
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private const string RecoveryResponseMessage = "Se este e-mail estiver cadastrado, as instruções de recuperação serão enviadas.";
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) => _authService = authService;

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequest)
        {
            try { return Ok(Success(await _authService.Login(loginRequest), "Login realizado com sucesso")); }
            catch (AuthValidationException ex) { return BadRequest(Invalid(ex.Errors, "Os dados informados para autenticação são inválidos")); }
            catch (AuthUnauthorizedException ex) { return Unauthorized(new Response { Code = ResponseEnum.UNAUTHORIZED, Data = null, Message = ex.Message }); }
            catch (Exception) { return StatusCode(500, new Response { Code = ResponseEnum.ERROR, Data = null, Message = "Não foi possível autenticar o usuário" }); }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDTO registerRequest)
        {
            try { return StatusCode(201, Success(await _authService.Register(registerRequest), "Cadastro realizado com sucesso")); }
            catch (UsuarioConflictException ex) { return Conflict(new Response { Code = ResponseEnum.CONFLICT, Data = new[] { ex.Field }, Message = ex.Message }); }
            catch (UsuarioValidationException ex) { return BadRequest(Invalid(ex.Errors, "Os dados informados para o cadastro são inválidos")); }
            catch (Exception) { return StatusCode(500, new Response { Code = ResponseEnum.ERROR, Data = null, Message = "Não foi possível concluir o cadastro" }); }
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestDTO request, CancellationToken cancellationToken)
        {
            try
            {
                await _authService.RequestPasswordRecovery(request, cancellationToken);
                return Ok(Success<object?>(null, RecoveryResponseMessage));
            }
            catch (AuthValidationException ex) { return BadRequest(Invalid(ex.Errors, "Os dados informados são inválidos")); }
            catch (Exception)
            {
                // Mantém a resposta indistinguível para não enumerar contas ou expor falhas SMTP.
                return Ok(Success<object?>(null, RecoveryResponseMessage));
            }
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestDTO request)
        {
            try
            {
                await _authService.ResetPassword(request);
                return Ok(Success<object?>(null, "Senha redefinida com sucesso"));
            }
            catch (InvalidPasswordResetTokenException ex) { return BadRequest(new Response { Code = ResponseEnum.INVALID, Data = null, Message = ex.Message }); }
            catch (AuthValidationException ex) { return BadRequest(Invalid(ex.Errors, "Os dados informados são inválidos")); }
            catch (Exception) { return StatusCode(500, new Response { Code = ResponseEnum.ERROR, Data = null, Message = "Não foi possível redefinir a senha" }); }
        }

        private static Response Success<T>(T data, string message) => new() { Code = ResponseEnum.SUCCESS, Data = data, Message = message };
        private static Response Invalid(IEnumerable<string> errors, string message) => new() { Code = ResponseEnum.INVALID, Data = errors, Message = message };
    }
}
