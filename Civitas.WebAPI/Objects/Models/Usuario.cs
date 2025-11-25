using Civitas.WebAPI.Objects.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Models
{
    [Table("usuario")]
    public class Usuario
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("cpf")]
        public string  Cpf { get; set; }

        [Column("nome")]
        public string Nome { get; set; }

        [Column("rg")]
        public string Rg { get; set; }

        [Column("logradouro")]
        public string Logradouro { get; set; }

        [Column("numero")]
        public string Numero { get; set; }

        [Column("cidade")]
        public string Cidade { get; set; }

        [Column("estado")]
        public string Estado { get; set; }

        [Column("cep")]
        public string Cep { get; set; }

        [Column("bairro")]
        public string Bairro { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("senha")]
        public string Senha { get; set; }

        [Column("matricula")]
        public string Matricula { get; set; }

        [Column("tipousuario")]
        public TipoUsuario TipoUsuario { get; set; }
        [Column("situacao")]
        public Situacao Situacao { get; set; }
        public ICollection<Despesa> Despesas { get; set; }

        public Usuario(int id, string cpf, string nome, string rg, string logradouro, string numero, string cidade, string estado, string cep, string email, string senha
            , Situacao situacao, string matricula, TipoUsuario tipoUsuario, string bairro)
        {
           Id = id;
           Cpf = cpf;
           Nome = nome;
           Rg = rg;
           Logradouro = logradouro;
           Numero = numero;
           Cidade = cidade;
           Estado = estado;
           Cep = cep;
           Email = email;
           Senha = senha;
           Situacao = situacao;
           Matricula = matricula;
           TipoUsuario = tipoUsuario;
           Bairro = bairro;
        }
    }
}
