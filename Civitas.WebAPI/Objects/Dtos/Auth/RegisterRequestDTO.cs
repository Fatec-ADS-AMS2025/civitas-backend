namespace Civitas.WebAPI.Objects.Dtos.Auth
{
    /// <summary>Dados aceitos no cadastro público. Perfil e situação são definidos pelo servidor.</summary>
    public class RegisterRequestDTO
    {
        public string Cpf { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Rg { get; set; } = string.Empty;
        public string Logradouro { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string Matricula { get; set; } = string.Empty;
    }
}
