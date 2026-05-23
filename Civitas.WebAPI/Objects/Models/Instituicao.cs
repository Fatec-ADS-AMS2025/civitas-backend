using Civitas.WebAPI.Objects.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Models
{
    /// <summary>
    /// Representa uma unidade administrativa, órgão público ou entidade ligada ao governo (escolas, hospitais, departamentos).
    /// Mapeia a tabela 'instituicao' do banco de dados.
    /// </summary>
    /// <remarks>
    /// A Instituição é a ponta executora do orçamento. Ela deve estar subordinada a uma <see cref="Secretaria"/> 
    /// e classificada por um <see cref="TipoInstituicao"/>.
    /// </remarks>
    [Table("instituicao")]
    public class Instituicao : ISoftDeletable
    {
        /// <summary>
        /// Identificador único da instituição (Chave Primária).
        /// </summary>
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Cadastro Nacional da Pessoa Jurídica.
        /// </summary>
        /// <remarks>
        /// Formato: 14 dígitos numéricos. Deve ser validado quanto à existência e formato (máscara).
        /// </remarks>
        [Column("cnpj")]
        public string CNPJ { get; set; }

        /// <summary>
        /// Nome Fantasia ou nome comum pelo qual a instituição é conhecida.
        /// </summary>
        /// <example>Escola Municipal Pequeno Príncipe, UPA Centro.</example>
        [Column("nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Nome da rua ou logradouro onde a instituição está situada.
        /// </summary>
        [Column("logradouro")]
        public string Logradouro { get; set; }

        /// <summary>
        /// Número do imóvel.
        /// </summary>
        [Column("numero")]
        public string Numero { get; set; }

        /// <summary>
        /// Bairro ou distrito de localização.
        /// </summary>
        [Column("bairro")]
        public string Bairro { get; set; }

        /// <summary>
        /// Código de Endereçamento Postal.
        /// </summary>
        [Column("cep")]
        public string CEP { get; set; }

        /// <summary>
        /// Razão Social oficial para fins jurídicos.
        /// </summary>
        [Column("nomerazaosocial")]
        public string NomeRazaoSocial { get; set; }

        /// <summary>
        /// Telefone de contato principal da unidade.
        /// </summary>
        [Column("telefone")]
        public string Telefone { get; set; }

        /// <summary>
        /// Endereço de e-mail institucional.
        /// </summary>
        [Column("email")]
        public string Email { get; set; }

        /// <summary>
        /// Cidade onde a instituição opera.
        /// </summary>
        [Column("cidade")]
        public string Cidade { get; set; }

        /// <summary>
        /// Unidade Federativa (UF).
        /// </summary>
        /// <example>SP, RJ, MG</example>
        [Column("estado")]
        public string Estado { get; set; }

        /// <summary>
        /// Situação cadastral da instituição.
        /// </summary>
        /// <remarks>
        /// Controlado pelo Enum <see cref="Situacao"/>.
        /// Instituições INATIVAS não podem receber novos orçamentos ou lançar despesas.
        /// </remarks>
        [Column("situacao")]
        public Situacao Situacao { get; set; }

        /// <summary>
        /// Chave estrangeira para o Tipo de Instituição (Categoria).
        /// </summary>
        [Column("idtipoinstituicao")]
        public int IdTipoInstituicao { get; set; }

        /// <summary>
        /// Objeto de navegação da categoria da instituição.
        /// </summary>
        public TipoInstituicao TipoInstituicao { get; set; }

        /// <summary>
        /// Chave estrangeira para a Secretaria à qual esta instituição responde.
        /// </summary>
        /// <remarks>
        /// Define a hierarquia administrativa (ex: Escola pertence à Secretaria de Educação).
        /// </remarks>
        [Column("idsecretaria")]
        public int IdSecretaria { get; set; }

        /// <summary>
        /// Objeto de navegação da Secretaria responsável.
        /// </summary>
        public Secretaria Secretaria { get; set; }

        /// <summary>
        /// Coleção de orçamentos disponíveis para esta instituição.
        /// </summary>
        public ICollection<Orcamento> Orcamento { get; set; }

        /// <summary>
        /// Propriedade de navegação para unidades consumidoras vinculadas.
        /// </summary>
        public ICollection<UnidadeConsumidora> UnidadesConsumidoras { get; set; }

        /// <summary>
        /// Construtor padrão para o Entity Framework.
        /// </summary>

        [Column("excluido")]
        public bool Excluido { get; set; }

        [Column("dataexclusao")]
        public DateTime? DataExclusao { get; set; }

        public Instituicao()
        {

        }

        /// <summary>
        /// Construtor completo para inicialização da Instituição.
        /// </summary>
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

