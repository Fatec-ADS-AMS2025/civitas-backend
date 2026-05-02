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

        /// <summary>
        /// Chave estrangeira da Institui��o detentora deste or�amento.
        /// </summary>
        [Column("idinstituicao")]
        public int IdInstituicao { get; set; }

        /// <summary>
        /// Objeto de navega��o para a Institui��o vinculada.
        /// </summary>
        public Instituicao? Instituicao { get; set; }

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
        public Orcamento(int idOrcamento, int anoOrcamento, decimal valorOrcamento, int idInstituicao)
        {
            IdOrcamento = idOrcamento;
            AnoOrcamento = anoOrcamento;
            ValorOrcamento = valorOrcamento;
            IdInstituicao = idInstituicao;
        }
    }
}
