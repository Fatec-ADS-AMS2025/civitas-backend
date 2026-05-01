using Civitas.WebAPI.Objects.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Models
{
    /// <summary>
    /// Entidade que define as categorias ou classificações de despesas permitidas no sistema.
    /// Mapeia a tabela 'tipodespesa' do banco de dados.
    /// </summary>
    /// <remarks>
    /// Esta entidade dita as regras de preenchimento da despesa, como a unidade de medida utilizada 
    /// e a obrigatoriedade de informar o código da Unidade Consumidora (UC).
    /// Exemplos: Energia Elétrica (Exige UC, Medido em kWh), Aluguel (Não exige UC, Unidade Fixa).
    /// </remarks>
    [Table("tipodespesa")]
    public class TipoDespesa
    {
        /// <summary>
        /// Identificador único do tipo de despesa (Chave Primária).
        /// </summary>
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Nome descritivo da categoria.
        /// </summary>
        /// <example>Energia Elétrica, Água, Internet, Manutenção Predial.</example>
        [Column("descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Chave estrangeira para a Unidade de Medida padrão deste tipo de despesa.
        /// </summary>
        [Column("idunidademedida")]
        public int IdUnidadeMedida { get; set; }

        /// <summary>
        /// Objeto de navegação da Unidade de Medida vinculada.
        /// </summary>
        public UnidadeMedida UnidadeMedida { get; set; }

        /// <summary>
        /// Define se o lançamento de uma despesa deste tipo exige o preenchimento da Unidade Consumidora (UC).
        /// </summary>
        /// <remarks>
        /// Regra de Negócio baseada no Enum <see cref="SolicitaUc"/>:
        /// 1 - Sim: O campo UC torna-se obrigatório no formulário de despesas (comum em concessionárias públicas).
        /// 2 - Não: O campo UC é opcional ou desabilitado.
        /// </remarks>
        [Column("solicitauc")]
        public SolicitaUc SolicitaUc { get; set; }

        /// <summary>
        /// Estado de ativação da categoria.
        /// </summary>
        /// <remarks>
        /// Baseado no Enum <see cref="Situacao"/>. Se INATIVO, não deve aparecer na lista de seleção para novas despesas.
        /// </remarks>
        [Column("situacao")]
        public Situacao Situacao { get; set; }

        [Column("idtipocodigo")]
        public int IdTipoCodigo { get; set; }

        public TipoCodigo TipoCodigo { get; set; }

        /// <summary>
        /// Coleção de unidades consumidoras vinculadas a este tipo de despesa.
        /// </summary>
        public ICollection<UnidadeConsumidora> UnidadesConsumidoras { get; set; }

        /// <summary>
        /// Construtor para inicialização da entidade TipoDespesa.
        /// </summary>
        public TipoDespesa(int id, string descricao, SolicitaUc solicitaUc, Situacao situacao)
        {
            Id = id;
            Descricao = descricao;
            SolicitaUc = solicitaUc;
            Situacao = situacao;
        }


    }
}
