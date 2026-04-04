using Civitas.WebAPI.Objects.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Models
{
    /// <summary>
    /// Entidade que representa um órgão governamental ou departamento administrativo superior (Secretaria).
    /// Mapeia a tabela 'secretaria' do banco de dados.
    /// </summary>
    /// <remarks>
    /// A Secretaria atua como entidade gestora, sendo responsável por um grupo de Instituições.
    /// Exemplo: Secretaria de Educação (gere Escolas), Secretaria de Saúde (gere Postos de Saúde).
    /// </remarks>
    [Table("secretaria")]
    public class Secretaria
    {
        /// <summary>
        /// Identificador único da secretaria (Chave Primária).
        /// </summary>
        [Column("idsecretaria")]
        public int IdSecretaria { get; set; }

        /// <summary>
        /// Situação cadastral da secretaria no sistema.
        /// </summary>
        /// <remarks>
        /// Controlado pelo Enum <see cref="Situacao"/>.
        /// Secretarias INATIVAS não devem permitir a abertura de novos processos ou vínculos de despesas.
        /// </remarks>
        [Column("situacao")]
        public Situacao Situacao { get; set; }

        /// <summary>
        /// Descrição detalhada ou missão da secretaria.
        /// </summary>
        [Column("descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Cadastro Nacional da Pessoa Jurídica.
        /// </summary>
        /// <remarks>
        /// Regra: Deve conter apenas números (14 dígitos) ou estar formatado com máscara padrão.
        /// Campo obrigatório para validações fiscais.
        /// </remarks>
        [Column("cnpj")]
        public string Cnpj { get; set; }

        /// <summary>
        /// Nome Fantasia ou nome de exibição pública da secretaria.
        /// </summary>
        /// <example>Secretaria Municipal de Educação</example>
        [Column("nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Nome da rua, avenida ou logradouro da sede administrativa.
        /// </summary>
        [Column("logradouro")]
        public string Logradouro { get; set; }

        /// <summary>
        /// Número do imóvel da sede.
        /// </summary>
        [Column("numero")]
        public string Numero { get; set; }

        /// <summary>
        /// Bairro ou distrito da sede.
        /// </summary>
        [Column("bairro")]
        public string Bairro { get; set; }

        /// <summary>
        /// Código de Endereçamento Postal da sede.
        /// </summary>
        [Column("cep")]
        public string Cep { get; set; }

        /// <summary>
        /// Razão Social oficial registrada para fins jurídicos e contratuais.
        /// </summary>
        [Column("nomerazaosocial")]
        public string NomeRazaoSocial { get; set; }

        /// <summary>
        /// E-mail oficial para contato administrativo.
        /// </summary>
        [Column("email")]
        public string Email { get; set; }

        /// <summary>
        /// Telefone fixo ou móvel da sede.
        /// </summary>
        [Column("telefone")]
        public string Telefone { get; set; }

        /// <summary>
        /// Cidade onde a secretaria está localizada.
        /// </summary>
        [Column("cidade")]
        public string Cidade { get; set; }

        /// <summary>
        /// Unidade Federativa (UF) da localização.
        /// </summary>
        [Column("estado")]
        public string Estado { get; set; }

        /// <summary>
        /// Relacionamento: Lista de instituições subordinadas a esta secretaria.
        /// </summary>
        public ICollection<Instituicao> Instituicoes { get; set; }

        /// <summary>
        /// Construtor vazio necessário para a materialização de objetos pelo Entity Framework.
        /// </summary>
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
        /// Construtor para inicialização completa da entidade Secretaria.
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