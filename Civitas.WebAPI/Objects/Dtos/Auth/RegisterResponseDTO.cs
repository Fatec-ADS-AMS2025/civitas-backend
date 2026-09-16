namespace Civitas.WebAPI.Objects.Dtos.Auth
{
    /// <summary>Resposta segura do cadastro público.</summary>
    public class RegisterResponseDTO
    {
        public int Id { get; init; }
        public string Nome { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
    }
}
