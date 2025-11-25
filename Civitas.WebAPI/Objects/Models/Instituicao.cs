using Civitas.WebAPI.Objects.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Models
{
    [Table("instituicao")]
    public class Instituicao
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("cnpj")]
        public string CNPJ { get; set; }

        [Column("nome")]
        public string Nome { get; set; }

        [Column("logradouro")]
        public string Logradouro { get; set; }

        [Column("numero")]
        public string Numero { get; set; }

        [Column("bairro")]
        public string Bairro { get; set; }

        [Column("cep")]
        public string CEP { get; set; }

        [Column("nomerazaosocial")]
        public string NomeRazaoSocial { get; set; }

        [Column("telefone")]
        public string Telefone { get; set; }
        
        [Column("email")]
        public string Email { get; set; }

        [Column("cidade")]
        public string Cidade { get; set; }

        [Column("estado")]
        public string Estado { get; set; }

        [Column("situacao")]
        public Situacao Situacao { get; set; }

        [Column("idtipoinstituicao")]
        public int IdTipoInstituicao { get; set; }

        public TipoInstituicao TipoInstituicao { get; set; }

        [Column("idsecretaria")]
        public int IdSecretaria { get; set; }

        public Secretaria Secretaria { get; set; }

        public ICollection<Orcamento> Orcamento { get; set; }

        public Despesa Despesa { get; set; }

        public Instituicao()
        {
            
        }

        public Instituicao(int id, string cnpj, string nome, string logradouro,
            string numero, string bairro, string cep, string nomerazaosocial, string telefone,
            string email, string cidade, string estado, Situacao situacao)
        {
            Id = id;
            CNPJ = cnpj;
            Nome = nome;
            Logradouro = logradouro;
            Numero = numero;
            Bairro = bairro;
            CEP = cep;
            NomeRazaoSocial = nomerazaosocial;
            Telefone = telefone;
            Email = email;
            Cidade = cidade;
            Estado = estado;
            Situacao = situacao;
        }
    }
}
