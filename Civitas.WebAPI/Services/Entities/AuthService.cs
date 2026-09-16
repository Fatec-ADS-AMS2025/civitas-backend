using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using Civitas.WebAPI.Data;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Auth;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;
using Civitas.WebAPI.Services.Security;
using Civitas.WebAPI.Services.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Civitas.WebAPI.Services.Entities
{
    public class AuthService : IAuthService
    {
        private const string PasswordRecoverySubject = "Redefinição de senha - Civitas";
        private static readonly EmailAddressAttribute EmailValidator = new();
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUsuarioService _usuarioService;
        private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
        private readonly IPasswordHashService _passwordHashService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IEmailSender _emailSender;
        private readonly AppDbContext _context;
        private readonly PasswordRecoveryOptions _passwordRecoveryOptions;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUsuarioRepository usuarioRepository, IUsuarioService usuarioService,
            IPasswordResetTokenRepository passwordResetTokenRepository, IPasswordHashService passwordHashService,
            IJwtTokenService jwtTokenService, IEmailSender emailSender, AppDbContext context,
            IOptions<PasswordRecoveryOptions> passwordRecoveryOptions, ILogger<AuthService> logger)
        {
            _usuarioRepository = usuarioRepository;
            _usuarioService = usuarioService;
            _passwordResetTokenRepository = passwordResetTokenRepository;
            _passwordHashService = passwordHashService;
            _jwtTokenService = jwtTokenService;
            _emailSender = emailSender;
            _context = context;
            _passwordRecoveryOptions = passwordRecoveryOptions.Value;
            _logger = logger;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequest)
        {
            ValidateLogin(loginRequest);
            var normalizedEmail = loginRequest.Email.Trim().ToLowerInvariant();
            var normalizedPassword = loginRequest.Senha.Trim();
            var usuario = await _usuarioRepository.GetByEmailAsync(normalizedEmail);
            if (usuario is null || usuario.Situacao != Situacao.ATIVO || !_passwordHashService.Verify(normalizedPassword, usuario.Senha))
            {
                throw new AuthUnauthorizedException("Credenciais inválidas.");
            }
            return _jwtTokenService.GenerateToken(usuario);
        }

        public async Task<RegisterResponseDTO> Register(RegisterRequestDTO registerRequest)
        {
            if (registerRequest is null) throw new UsuarioValidationException(["O corpo da requisição é obrigatório."]);

            var usuario = new UsuarioDTO
            {
                Cpf = registerRequest.Cpf, Nome = registerRequest.Nome, Rg = registerRequest.Rg,
                Logradouro = registerRequest.Logradouro, Numero = registerRequest.Numero, Bairro = registerRequest.Bairro,
                Cidade = registerRequest.Cidade, Estado = registerRequest.Estado, Cep = registerRequest.Cep,
                Email = registerRequest.Email, Senha = registerRequest.Senha, Matricula = registerRequest.Matricula,
                Situacao = Situacao.ATIVO, TipoUsuario = TipoUsuario.VISITANTE
            };
            await _usuarioService.Create(usuario);
            return new RegisterResponseDTO { Id = usuario.Id, Nome = usuario.Nome, Email = usuario.Email };
        }

        public async Task RequestPasswordRecovery(ForgotPasswordRequestDTO request, CancellationToken cancellationToken = default)
        {
            if (request is null || !IsValidEmail(request.Email, out var email))
                throw new AuthValidationException(["O campo Email é inválido."]);

            var usuario = await _usuarioRepository.GetByEmailAsync(email);
            if (usuario is null || usuario.Situacao != Situacao.ATIVO) return;

            var now = DateTime.UtcNow;
            var rawToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            var token = new PasswordResetToken
            {
                UsuarioId = usuario.Id, TokenHash = HashToken(rawToken), CriadoEm = now,
                ExpiraEm = now.AddMinutes(GetExpirationMinutes())
            };

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            await _passwordResetTokenRepository.InvalidateUnusedForUserAsync(usuario.Id, now);
            await _passwordResetTokenRepository.AddAsync(token);
            await transaction.CommitAsync(cancellationToken);

            try
            {
                var recoveryLink = $"{_passwordRecoveryOptions.FrontendBaseUrl.TrimEnd('/')}/reset-password?token={Uri.EscapeDataString(rawToken)}";
                await _emailSender.SendAsync(email, PasswordRecoverySubject, $"Use este link para redefinir sua senha: {recoveryLink}", cancellationToken);
            }
            catch (Exception)
            {
                await _passwordResetTokenRepository.TryConsumeAsync(token.Id, DateTime.UtcNow);
                _logger.LogWarning("Password recovery email delivery failed.");
            }
        }

        public async Task ResetPassword(ResetPasswordRequestDTO request)
        {
            ValidateResetRequest(request);
            var now = DateTime.UtcNow;
            var token = await _passwordResetTokenRepository.GetActiveByHashAsync(HashToken(request.Token.Trim()), now);
            if (token is null) throw new InvalidPasswordResetTokenException();

            await using var transaction = await _context.Database.BeginTransactionAsync();
            if (!await _passwordResetTokenRepository.TryConsumeAsync(token.Id, now)) throw new InvalidPasswordResetTokenException();

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(item => item.Id == token.UsuarioId && !item.Excluido);
            if (usuario is null) throw new InvalidPasswordResetTokenException();

            usuario.Senha = _passwordHashService.Hash(request.Senha.Trim());
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        private static void ValidateLogin(LoginRequestDTO? request)
        {
            if (request is null) throw new AuthValidationException(["O corpo da requisição é obrigatório."]);
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(request.Email)) errors.Add("O campo Email é obrigatório.");
            if (string.IsNullOrWhiteSpace(request.Senha)) errors.Add("O campo Senha é obrigatório.");
            if (errors.Count > 0) throw new AuthValidationException(errors);
        }

        private static void ValidateResetRequest(ResetPasswordRequestDTO? request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Token)) throw new InvalidPasswordResetTokenException();
            var senha = request.Senha?.Trim() ?? string.Empty;
            var errors = new List<string>();
            if (senha.Length < 8) errors.Add("A senha deve ter no mínimo 8 caracteres.");
            if (!senha.Any(char.IsLetter) || !senha.Any(char.IsDigit)) errors.Add("A senha deve conter pelo menos uma letra e um número.");
            if (errors.Count > 0) throw new AuthValidationException(errors);
        }

        private static bool IsValidEmail(string? value, out string email)
        {
            email = value?.Trim().ToLowerInvariant() ?? string.Empty;
            return email.Length <= 255 && !email.Any(char.IsWhiteSpace) && EmailValidator.IsValid(email);
        }

        private int GetExpirationMinutes() => Math.Max(_passwordRecoveryOptions.TokenExpirationMinutes, 1);
        private static string HashToken(string token) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }
}
