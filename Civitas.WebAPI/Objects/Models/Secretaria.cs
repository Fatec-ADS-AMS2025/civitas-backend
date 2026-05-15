using Civitas.WebAPI.Objects.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Models
{
    /// <summary>
    /// Entidade que representa um Ã³rgÃ£o governamental ou departamento administrativo superior (Secretaria).
    /// Mapeia a tabela 'secretaria' do banco de dados.
    /// </summary>
    /// <remarks>
    /// A Secretaria atua como entidade gestora, sendo responsÃ¡vel por um grupo de InstituiÃ§Ãµes.
    /// Exemplo: Secretaria de EducaÃ§Ã£o (gere Escolas), Secretaria de SaÃºde (gere Postos de SaÃºde).
    /// </remarks>
    [Table("secretaria")]
    public class Secretaria : ISoftDeletable
    {
        /// <summary>
        /// Identificador Ãºnico da secretaria (Chave PrimÃ¡ria).
        /// </summary>
        [Column("idsecretaria")]
        public int IdSecretaria { get; set; }

        /// <summary>
        /// SituaÃ§Ã£o cadastral da secretaria no sistema.
        /// </summary>
        /// <remarks>
        /// Controlado pelo Enum <see cref="Situacao"/>.
        /// Secretarias INATIVAS nÃ£o devem permitir a abertura de novos processos ou vÃ­nculos de despesas.
        /// </remarks>
        [Column("situacao")]
        public Situacao Situacao { get; set; }

        /// <summary>
        /// DescriÃ§Ã£o detalhada ou missÃ£o da secretaria.
        /// </summary>
        [Column("descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Cadastro Nacional da Pessoa JurÃ­dica.
        /// </summary>
        /// <remarks>
        /// Regra: Deve conter apenas nÃºmeros (14 dÃ­gitos) ou estar formatado com mÃ¡scara padrÃ£o.
        /// Campo obrigatÃ³rio para validaÃ§Ãµes fiscais.
        /// </remarks>
        [Column("cnpj")]
        public string Cnpj { get; set; }

        /// <summary>
        /// Nome Fantasia ou nome de exibiÃ§Ã£o pÃºblica da secretaria.
        /// </summary>
        /// <example>Secretaria Municipal de EducaÃ§Ã£o</example>
        [Column("nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Nome da rua, avenida ou logradouro da sede administrativa.
        /// </summary>
        [Column("logradouro")]
        public string Logradouro { get; set; }

        /// <summary>
        /// NÃºmero do imÃ³vel da sede.
        /// </summary>
        [Column("numero")]
        public string Numero { get; set; }

        /// <summary>
        /// Bairro ou distrito da sede.
        /// </summary>
        [Column("bairro")]
        public string Bairro { get; set; }

        /// <summary>
        /// CÃ³digo de EndereÃ§amento Postal da sede.
        /// </summary>
        [Column("cep")]
        public string Cep { get; set; }

        /// <summary>
        /// RazÃ£o Social oficial registrada para fins jurÃ­dicos e contratuais.
        /// </summary>
        [Column("nomerazaosocial")]
        public string NomeRazaoSocial { get; set; }

        /// <summary>
        /// E-mail oficial para contato administrativo.
        /// </summary>
        [Column("email")]
        public string Email { get; set; }

        /// <summary>
        /// Telefone fixo ou mÃ³vel da sede.
        /// </summary>
        [Column("telefone")]
        public string Telefone { get; set; }

        /// <summary>
        /// Cidade onde a secretaria estÃ¡ localizada.
        /// </summary>
        [Column("cidade")]
        public string Cidade { get; set; }

        /// <summary>
        /// Unidade Federativa (UF) da localizaÃ§Ã£o.
        /// </summary>
        [Column("estado")]
        public string Estado { get; set; }

        /// <summary>
        /// Relacionamento: Lista de instituiÃ§Ãµes subordinadas a esta secretaria.
        /// </summary>
        public ICollection<Instituicao> Instituicoes { get; set; }

        /// <summary>
        /// Construtor vazio necessÃ¡rio para a materializaÃ§Ã£o de objetos pelo Entity Framework.
        /// </summary>

        [Column("excluido")]
        public bool Excluido { get; set; }

        [Column("dataexclusao")]
        public DateTime? DataExclusao { get; set; }

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

        /// <summary>
        /// Construtor para inicializaÃ§Ã£o completa da entidade Secretaria.
        /// </summary>
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
