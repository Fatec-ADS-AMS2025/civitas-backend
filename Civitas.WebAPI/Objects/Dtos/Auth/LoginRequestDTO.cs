namespace Civitas.WebAPI.Objects.Dtos.Auth
{
    /// <summary>
    /// Payload de autenticação utilizado para realizar login na API.
    /// </summary>
    public class LoginRequestDTO
    {
        /// <summary>
        /// E-mail do usuário.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Senha em texto plano informada no login.
        /// </summary>
        public string Senha { get; set; } = string.Empty;
    }
}
