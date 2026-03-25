using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Auth;
using Civitas.WebAPI.Services.Interfaces;
using Civitas.WebAPI.Services.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Controllers
{
    /// <summary>
    /// Controlador responsável pela autenticação de usuários.
    /// </summary>
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly Response _response;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
            _response = new Response();
        }

        /// <summary>
        /// Autentica um usuário com e-mail e senha e retorna um token JWT.
        /// </summary>
        /// <param name="loginRequest">Credenciais do usuário.</param>
        /// <returns>Token JWT e dados essenciais do usuário autenticado.</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequest)
        {
            try
            {
                var loginResponse = await _authService.Login(loginRequest);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = loginResponse;
                _response.Message = "Login realizado com sucesso";

                return Ok(_response);
            }
            catch (AuthValidationException ex)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = ex.Errors;
                _response.Message = "Os dados informados para autenticação são inválidos";
                return BadRequest(_response);
            }
            catch (AuthUnauthorizedException ex)
            {
                _response.Code = ResponseEnum.UNAUTHORIZED;
                _response.Data = null;
                _response.Message = ex.Message;
                return Unauthorized(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Não foi possível autenticar o usuário";
                _response.Data = new
                {
                    ErrorMessage = ex.Message
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }
    }
}
