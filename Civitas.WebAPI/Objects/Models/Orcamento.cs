using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Models
{
    /// <summary>
    /// Entidade que define a previsão orçamentária (Teto de Gastos) disponível para uma instituição.
    /// Mapeia a tabela 'orcamento' do banco de dados.
    /// </summary>
    /// <remarks>
    /// O orçamento é segmentado por Ano, Instituição e Tipo de Despesa.
    /// Regra de Negócio: O sistema deve validar se a soma das despesas lançadas ultrapassa este ValorOrcamento.
    /// </remarks>
    [Table("orcamento")]
    public class Orcamento
    {
        /// <summary>
        /// Identificador único do registro de orçamento (Chave Primária).
        /// </summary>
        [Column("idorcamento")]
        public int IdOrcamento { get; set; }

        /// <summary>
        /// Ano de exercício fiscal ao qual este orçamento pertence.
        /// </summary>
        /// <example>2024, 2025</example>
        /// <remarks>
        /// Regra: Deve ser um ano válido com 4 dígitos.
        /// </remarks>
        [Column("anoorcamento")]
        public int AnoOrcamento { get; set; }

        /// <summary>
        /// Valor monetário total disponibilizado para gastos.
        /// </summary>
        /// <remarks>
        /// Este valor serve como limite superior para as validações de despesas.
        /// </remarks>
        [Column("valororcamento")]
        public double ValorOrcamento { get; set; }

        /// <summary>
        /// Chave estrangeira da Instituição detentora deste orçamento.
        /// </summary>
        [Column("idinstituicao")]
        public int IdInstituicao { get; set; }

        /// <summary>
        /// Objeto de navegação para a Instituição vinculada.
        /// </summary>
        public Instituicao? Instituicao { get; set; }

        /// <summary>
        /// Chave estrangeira do Tipo de Despesa que este orçamento cobre.
        /// </summary>
        /// <remarks>
        /// Exemplo: Um orçamento específico apenas para "Energia Elétrica" ou "Material de Escritório".
        /// </remarks>
        [Column("idtipodespesa")]
        public int IdTipoDespesa { get; set; }

        /// <summary>
        /// Objeto de navegação para o Tipo de Despesa vinculado.
        /// </summary>
        public TipoDespesa? TipoDespesa { get; set; }

        /// <summary>
        /// Relacionamento: Lista de despesas reais que estão consumindo este orçamento.
        /// </summary>
        public ICollection<Despesa> Despesas { get; set; }

        /// <summary>
        /// Construtor vazio para uso do Entity Framework.
        /// </summary>
        public Orcamento()
        {

        }

        /// <summary>
        /// Construtor para inicialização básica da entidade Orcamento.
        /// </summary>
        public Orcamento(int idOrcamento, int anoOrcamento, double valorOrcamento, int idInstituicao)
        {
            IdOrcamento = idOrcamento;
            AnoOrcamento = anoOrcamento;
            ValorOrcamento = valorOrcamento;
            IdInstituicao = idInstituicao;
        }
    }
}