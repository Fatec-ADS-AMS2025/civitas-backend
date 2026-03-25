namespace Civitas.WebAPI.Objects.Dtos.Auth
{
    /// <summary>
    /// Resposta de sucesso da autenticação com token e dados básicos do usuário.
    /// </summary>
    public class LoginResponseDTO
    {
        /// <summary>
        /// Token JWT emitido pela API.
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Momento UTC de expiração do token.
        /// </summary>
        public DateTime ExpiresAtUtc { get; set; }

        /// <summary>
        /// Dados essenciais do usuário autenticado.
        /// </summary>
        public LoginUsuarioDTO Usuario { get; set; } = new();
    }

    /// <summary>
    /// Identificação mínima do usuário autenticado retornada no login.
    /// </summary>
    public class LoginUsuarioDTO
    {
        /// <summary>
        /// Identificador do usuário.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome do usuário.
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Tipo de usuário.
        /// </summary>
        public string TipoUsuario { get; set; } = string.Empty;
    }
}
