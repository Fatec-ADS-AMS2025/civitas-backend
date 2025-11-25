using Civitas.WebAPI.Objects.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Models
{
    [Table("secretaria")]
    public class Secretaria
    {
        [Column("idsecretaria")]
        public int IdSecretaria { get; set; }

        [Column("situacao")]
        public Situacao Situacao { get; set; }

        [Column("descricao")]
        public string Descricao { get; set; }

        [Column("cnpj")]
        public string Cnpj { get; set; }

        [Column("nome")]
        public string Nome { get; set; }

        [Column("logradouro")]
        public string Logradouro { get; set; }

        [Column("numero")]
        public string Numero { get; set; }

        [Column("bairro")]
        public string Bairro { get; set; }

        [Column("cep")]
        public string Cep { get; set; }

        [Column("nomerazaosocial")]
        public string NomeRazaoSocial { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("telefone")]
        public string Telefone { get; set; }

        [Column("cidade")]
        public string Cidade { get; set; }

        [Column("estado")]
        public string Estado { get; set; }

        public ICollection<Instituicao> Instituicoes { get; set; }

        // Construtor parameterless necessário para o Entity Framework
        public Secretaria()
        {
            Descricao = string.Empty;
            Cnpj = string.Empty;
            Nome = string.Empty;
            Logradouro = string.Empty;
            Numero = string.Empty;
            Bairro = string.Empty;
            Cep = string.Empty;
            NomeRazaoSocial = string.Empty;
            Email = string.Empty;
            Telefone = string.Empty;
            Cidade = string.Empty;
            Estado = string.Empty;
        }

        public Secretaria(int idSecretaria, Situacao situacao, string descricao, string cnpj, string nome, 
            string logradouro, string numero, string bairro, string cep, string nomeRazaoSocial, 
            string email, string telefone, string cidade, string estado)
        {
            IdSecretaria = idSecretaria;
            Situacao = situacao;
            Descricao = descricao;
            Cnpj = cnpj;
            Nome = nome;
            Logradouro = logradouro;
            Numero = numero;
            Bairro = bairro;
            Cep = cep;
            NomeRazaoSocial = nomeRazaoSocial;
            Email = email;
            Telefone = telefone;
            Cidade = cidade;
            Estado = estado;
        }
    }
}
