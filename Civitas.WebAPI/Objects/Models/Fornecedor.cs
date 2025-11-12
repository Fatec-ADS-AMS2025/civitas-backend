using Civitas.WebAPI.Objects.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Models
{
    [Table("fornecedor")]
    public class Fornecedor
    {
        [Column("idfornecedor")]
        public int IdFornecedor { get; set; }

        [Column("nomefantasia")]
        public string NomeFantasia { get; set; }

        [Column("situacao")]
        public Situacao Situacao { get; set; }

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

        [Column("telefone")]
        public string Telefone { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("cidade")]
        public string Cidade { get; set; }

        [Column("estado")]
        public string Estado { get; set; }

        public ICollection<Documento> Documentos { get; set; }
        public ICollection<Despesa> Despesas { get; set; }

        public Fornecedor(int idFornecedor, string nomeFantasia, Situacao situacao, string cnpj, string nome, string logradouro, string numero, string bairro, string cep
            , string telefone, string email, string cidade, string estado)
        {
            IdFornecedor = idFornecedor;
            NomeFantasia = nomeFantasia;
            Situacao = situacao;
            Cnpj = cnpj;
            Nome = nome;
            Logradouro = logradouro;
            Numero = numero;
            Bairro = bairro;
            Cep = cep;
            Telefone = telefone;
            Email = email;
            Cidade = cidade;
            Estado = estado;
        }
    }
}
