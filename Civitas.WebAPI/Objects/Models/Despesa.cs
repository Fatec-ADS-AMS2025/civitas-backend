using Civitas.WebAPI.Objects.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Models
{
    /// <summary>
    /// Entidade central que representa uma obrigação financeira, conta ou fatura a ser paga.
    /// Mapeia a tabela 'despesa' do banco de dados.
    /// </summary>
    /// <remarks>
    /// A Despesa conecta a execução financeira (Instituição) ao planejamento (Orçamento) e ao credor (Fornecedor).
    /// É o registro "pai" dos pagamentos que serão efetuados na entidade Fluxo.
    /// </remarks>
    [Table("despesa")]
    public class Despesa
    {
        /// <summary>
        /// Identificador único da despesa (Chave Primária).
        /// </summary>
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Número de identificação impresso no documento fiscal (NF, Fatura, Boleto).
        /// </summary>
        /// <remarks>
        /// Usado para rastreabilidade e para evitar pagamentos em duplicidade.
        /// </remarks>
        [Column("numerodocumento")]
        public string NumeroDocumento { get; set; }

        /// <summary>
        /// Código da Unidade Consumidora (UC) ou Instalação.
        /// </summary>
        /// <remarks>
        /// Regra de Negócio: A obrigatoriedade deste campo é definida pelo <see cref="TipoDespesa"/> vinculado.
        /// Se TipoDespesa.SolicitaUc for 'Sim', este campo é mandatório (comum em contas de Energia/Água).
        /// </remarks>
        [Column("uc")]
        public string UC { get; set; }

        /// <summary>
        /// Data em que o documento foi emitido pelo fornecedor.
        /// </summary>
        /// <remarks>
        /// Formato esperado: String (ex: DD/MM/AAAA) ou ISO 8601.
        /// </remarks>
        [Column("dataemissao")]
        public string DataEmicao { get; set; }

        /// <summary>
        /// Quantidade estimada de consumo para esta despesa.
        /// </summary>
        /// <remarks>
        /// A unidade de medida (kWh, m³, etc) é determinada pelo <see cref="TipoDespesa"/>.
        /// Importante para relatórios de previsão versus realizado.
        /// </remarks>
        [Column("consumoprevisto")]
        public double ConsumoPrevisto { get; set; }

        /// <summary>
        /// Data limite para o pagamento sem incidência de juros ou multas.
        /// </summary>
        [Column("datavencimento")]
        public DateOnly DataVencimento { get; set; }

        /// <summary>
        /// Estado de ativação do registro de despesa.
        /// </summary>
        /// <remarks>
        /// Controlado pelo Enum <see cref="Situacao"/>. 
        /// Despesas INATIVAS (excluídas logicamente) não devem entrar nos relatórios de soma orçamentária.
        /// </remarks>
        [Column("situacao")]
        public Situacao Situacao { get; set; }

        /// <summary>
        /// Chave estrangeira para a Categoria da Despesa.
        /// </summary>
        [Column("idtipodespesa")]
        public int IdTipoDespesa { get; set; }

        /// <summary>
        /// Objeto de navegação do Tipo de Despesa. Define regras como Unidade de Medida e uso de UC.
        /// </summary>
        public TipoDespesa TipoDespesa { get; set; }

        /// <summary>
        /// Chave estrangeira do Orçamento que cobrirá este gasto.
        /// </summary>
        [Column("idorcamento")]
        public int IdOrcamento { get; set; }

        /// <summary>
        /// Objeto de navegação do Orçamento.
        /// </summary>
        /// <remarks>
        /// Regra: O valor desta despesa deve ser abatido do saldo deste Orçamento.
        /// </remarks>
        public Orcamento Orcamento { get; set; }

        /// <summary>
        /// Chave estrangeira da Instituição responsável pelo pagamento.
        /// </summary>
        [Column("idinstituicao")]
        public int IdInstituicao { get; set; }

        /// <summary>
        /// Objeto de navegação da Instituição.
        /// </summary>
        public Instituicao Instituicao { get; set; }

        /// <summary>
        /// Chave estrangeira do Fornecedor que receberá o pagamento.
        /// </summary>
        [Column("idfornecedor")]
        public int IdFornecedor { get; set; }

        /// <summary>
        /// Objeto de navegação do Fornecedor.
        /// </summary>
        public Fornecedor Fornecedor { get; set; }

        /// <summary>
        /// Chave estrangeira do Usuário que cadastrou esta despesa.
        /// </summary>
        [Column("idusuario")]
        public int IdUsuario { get; set; }

        /// <summary>
        /// Objeto de navegação do Usuário (Audit Trail).
        /// </summary>
        public Usuario Usuario { get; set; }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public Despesa()
        {

        }

        /// <summary>
        /// Construtor para inicialização dos dados principais da Despesa.
        /// </summary>
        public Despesa(int id, string numeroDocumento, string uc, string dataEmicao, double consumoPrevisto, DateOnly dataVencimento, Situacao situacao)
        {
            Id = id;
            NumeroDocumento = numeroDocumento;
            UC = uc;
            DataEmicao = dataEmicao;
            ConsumoPrevisto = consumoPrevisto;
            DataVencimento = dataVencimento;
            Situacao = situacao;
        }
    }
}