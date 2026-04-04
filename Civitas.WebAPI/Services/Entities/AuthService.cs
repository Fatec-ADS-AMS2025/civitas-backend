using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Auth;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Services.Interfaces;
using Civitas.WebAPI.Services.Security;
using Civitas.WebAPI.Services.Validation;

namespace Civitas.WebAPI.Services.Entities
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHashService _passwordHashService;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(
            IUsuarioRepository usuarioRepository,
            IPasswordHashService passwordHashService,
            IJwtTokenService jwtTokenService)
        {
            _usuarioRepository = usuarioRepository;
            _passwordHashService = passwordHashService;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequest)
        {
            Validate(loginRequest);

            var normalizedEmail = loginRequest.Email.Trim().ToLowerInvariant();
            var normalizedPassword = loginRequest.Senha.Trim();

            var usuario = await _usuarioRepository.GetByEmailAsync(normalizedEmail);
            if (usuario is null ||
                usuario.Situacao != Situacao.ATIVO ||
                !_passwordHashService.Verify(normalizedPassword, usuario.Senha))
            {
                throw new AuthUnauthorizedException("Credenciais inválidas.");
            }

            return _jwtTokenService.GenerateToken(usuario);
        }

        private static void Validate(LoginRequestDTO? loginRequest)
        {
            if (loginRequest is null)
            {
                throw new AuthValidationException(["O corpo da requisição é obrigatório."]);
            }

            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(loginRequest.Email))
            {
                errors.Add("O campo Email é obrigatório.");
            }

            if (string.IsNullOrWhiteSpace(loginRequest.Senha))
            {
                errors.Add("O campo Senha é obrigatório.");
            }

            if (errors.Count > 0)
            {
                throw new AuthValidationException(errors);
            }
        }
    }
}
