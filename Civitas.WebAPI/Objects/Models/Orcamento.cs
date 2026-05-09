using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Models
{
    /// <summary>
    /// Entidade que define a previs�o or�ament�ria (Teto de Gastos) dispon�vel para uma institui��o.
    /// Mapeia a tabela 'orcamento' do banco de dados.
    /// </summary>
    /// <remarks>
    /// O or�amento � segmentado por Ano, Institui��o e Tipo de Despesa.
    /// Regra de Neg�cio: O sistema deve validar se a soma das despesas lan�adas ultrapassa este ValorOrcamento.
    /// </remarks>
    [Table("orcamento")]
    public class Orcamento
    {
        /// <summary>
        /// Identificador �nico do registro de or�amento (Chave Prim�ria).
        /// </summary>
        [Column("idorcamento")]
        public int IdOrcamento { get; set; }

        /// <summary>
        /// Ano de exerc�cio fiscal ao qual este or�amento pertence.
        /// </summary>
        /// <example>2024, 2025</example>
        /// <remarks>
        /// Regra: Deve ser um ano v�lido com 4 d�gitos.
        /// </remarks>
        [Column("anoorcamento")]
        public int AnoOrcamento { get; set; }

        /// <summary>
        /// Valor monet�rio total disponibilizado para gastos.
        /// </summary>
        /// <remarks>
        /// Este valor serve como limite superior para as valida��es de despesas.
        /// </remarks>
        [Column("valororcamento")]
        public decimal ValorOrcamento { get; set; }


        [Column("janeiroquantidadeconsumo")]
        public decimal? JaneiroQuantidadeConsumo { get; set; }

        [Column("janeirovalorconsumo")]
        public decimal? JaneiroValorConsumo { get; set; }

        [Column("fevereiroquantidadeconsumo")]
        public decimal? FevereiroQuantidadeConsumo { get; set; }

        [Column("fevereirovalorconsumo")]
        public decimal? FevereiroValorConsumo { get; set; }

        [Column("marcoquantidadeconsumo")]
        public decimal? MarcoQuantidadeConsumo { get; set; }

        [Column("marcovalorconsumo")]
        public decimal? MarcoValorConsumo { get; set; }

        [Column("abrilquantidadeconsumo")]
        public decimal? AbrilQuantidadeConsumo { get; set; }

        [Column("abrilvalorconsumo")]
        public decimal? AbrilValorConsumo { get; set; }

        [Column("maioquantidadeconsumo")]
        public decimal? MaioQuantidadeConsumo { get; set; }

        [Column("maiovalorconsumo")]
        public decimal? MaioValorConsumo { get; set; }

        [Column("junhoquantidadeconsumo")]
        public decimal? JunhoQuantidadeConsumo { get; set; }

        [Column("junhovalorconsumo")]
        public decimal? JunhoValorConsumo { get; set; }

        [Column("julhoquantidadeconsumo")]
        public decimal? JulhoQuantidadeConsumo { get; set; }

        [Column("julhovalorconsumo")]
        public decimal? JulhoValorConsumo { get; set; }

        [Column("agostoquantidadeconsumo")]
        public decimal? AgostoQuantidadeConsumo { get; set; }

        [Column("agostovalorconsumo")]
        public decimal? AgostoValorConsumo { get; set; }

        [Column("setembroquantidadeconsumo")]
        public decimal? SetembroQuantidadeConsumo { get; set; }

        [Column("setembrovalorconsumo")]
        public decimal? SetembroValorConsumo { get; set; }

        [Column("outubroquantidadeconsumo")]
        public decimal? OutubroQuantidadeConsumo { get; set; }

        [Column("outubrovalorconsumo")]
        public decimal? OutubroValorConsumo { get; set; }

        [Column("novembroquantidadeconsumo")]
        public decimal? NovembroQuantidadeConsumo { get; set; }

        [Column("novembrovalorconsumo")]
        public decimal? NovembroValorConsumo { get; set; }

        [Column("dezembroquantidadeconsumo")]
        public decimal? DezembroQuantidadeConsumo { get; set; }

        [Column("dezembrovalorconsumo")]
        public decimal? DezembroValorConsumo { get; set; }

        /// <summary>
        /// Chave estrangeira da Institui��o detentora deste or�amento.
        /// </summary>
        [Column("idinstituicao")]
        public int IdInstituicao { get; set; }

        /// <summary>
        /// Objeto de navega��o para a Institui��o vinculada.
        /// </summary>
        public Instituicao? Instituicao { get; set; }


        [Column("idtipodespesa")]
        public int IdTipoDespesa { get; set; }

        public TipoDespesa TipoDespesa { get; set; }
        /// <summary>
        /// Relacionamento: unidades consumidoras que utilizam este orcamento.
        /// </summary>
        public ICollection<UnidadeConsumidora> UnidadesConsumidoras { get; set; }

        /// <summary>
        /// Construtor vazio para uso do Entity Framework.
        /// </summary>
        public Orcamento()
        {

        }

        /// <summary>
        /// Construtor para inicializa��o b�sica da entidade Orcamento.
        /// </summary>
        public Orcamento(int idOrcamento, int anoOrcamento, decimal valorOrcamento, int idInstituicao, int idTipoDespesa, decimal? janeiroQuantidadeConsumo,
                decimal? janeiroValorConsumo, decimal? fevereiroQuantidadeConsumo, decimal? fevereiroValorConsumo, decimal? marcoQuantidadeConsumo, decimal? marcoValorConsumo,
                decimal? abrilQuantidadeConsumo,decimal? abrilValorConsumo,decimal? maioQuantidadeConsumo,decimal? maioValorConsumo,decimal? junhoQuantidadeConsumo,
                decimal? junhoValorConsumo, decimal? julhoQuantidadeConsumo,decimal? julhoValorConsumo, decimal? agostoQuantidadeConsumo, decimal? agostoValorConsumo, decimal? setembroQuantidadeConsumo,
                decimal? setembroValorConsumo, decimal? outubroQuantidadeConsumo,decimal? outubroValorConsumo,decimal? novembroQuantidadeConsumo,
                decimal? novembroValorConsumo, decimal? dezembroQuantidadeConsumo,decimal? dezembroValorConsumo
        )
        {
            IdOrcamento = idOrcamento;
            AnoOrcamento = anoOrcamento;
            ValorOrcamento = valorOrcamento;
            IdInstituicao = idInstituicao;
            IdTipoDespesa = idTipoDespesa;

            JaneiroQuantidadeConsumo = janeiroQuantidadeConsumo;
            JaneiroValorConsumo = janeiroValorConsumo;

            FevereiroQuantidadeConsumo = fevereiroQuantidadeConsumo;
            FevereiroValorConsumo = fevereiroValorConsumo;

            MarcoQuantidadeConsumo = marcoQuantidadeConsumo;
            MarcoValorConsumo = marcoValorConsumo;

            AbrilQuantidadeConsumo = abrilQuantidadeConsumo;
            AbrilValorConsumo = abrilValorConsumo;

            MaioQuantidadeConsumo = maioQuantidadeConsumo;
            MaioValorConsumo = maioValorConsumo;

            JunhoQuantidadeConsumo = junhoQuantidadeConsumo;
            JunhoValorConsumo = junhoValorConsumo;

            JulhoQuantidadeConsumo = julhoQuantidadeConsumo;
            JulhoValorConsumo = julhoValorConsumo;

            AgostoQuantidadeConsumo = agostoQuantidadeConsumo;
            AgostoValorConsumo = agostoValorConsumo;

            SetembroQuantidadeConsumo = setembroQuantidadeConsumo;
            SetembroValorConsumo = setembroValorConsumo;

            OutubroQuantidadeConsumo = outubroQuantidadeConsumo;
            OutubroValorConsumo = outubroValorConsumo;

            NovembroQuantidadeConsumo = novembroQuantidadeConsumo;
            NovembroValorConsumo = novembroValorConsumo;

            DezembroQuantidadeConsumo = dezembroQuantidadeConsumo;
            DezembroValorConsumo = dezembroValorConsumo;
        }
    }
}
