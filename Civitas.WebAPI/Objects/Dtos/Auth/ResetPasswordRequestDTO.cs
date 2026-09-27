namespace Civitas.WebAPI.Objects.Dtos.Auth
{
    public class ResetPasswordRequestDTO
    {
        public string Token { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }
}
