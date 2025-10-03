using Civitas.WebAPI.Objects.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    public class UsuarioDTO
    {
        public int Id { get; set; }
        public string Cpf { get; set; }
        public string Nome { get; set; }
        public string Rg { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Cep { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public Situacao Situacao { get; set; }
        public string Matricula { get; set; }

        public TipoUsuario TipoUsuario { get; set; }
        public string Bairro { get; set; }
    }
}
