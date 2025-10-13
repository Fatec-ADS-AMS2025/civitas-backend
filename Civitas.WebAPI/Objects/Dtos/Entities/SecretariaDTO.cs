using Civitas.WebAPI.Objects.Enums;

namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    public class SecretariaDTO
    {
        public int IdSecretaria { get; set; }
        public Situacao Situacao { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Logradouro { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string NomeRazaoSocial { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }
}
