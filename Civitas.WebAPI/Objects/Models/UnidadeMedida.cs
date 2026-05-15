using Civitas.WebAPI.Objects.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Models
{
    /// <summary>
    /// Entidade que representa as Unidades de Medida utilizadas no sistema para quantificar despesas ou consumos.
    /// Mapeia a tabela 'unidademedida' do banco de dados.
    /// </summary>
    /// <remarks>
    /// Exemplos de uso: Quilowatts (kW) para energia, Metros Cúbicos (m³) para água, Unidade (un) para itens fixos.
    /// </remarks>
    [Table("unidademedida")]
    public class UnidadeMedida : ISoftDeletable
    {
        /// <summary>
        /// Identificador único da unidade de medida (Chave Primária).
        /// </summary>
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Descrição completa do nome da unidade.
        /// </summary>
        /// <example>Quilowatts-hora, Litros, Metros Cúbicos.</example>
        [Column("descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Símbolo ou sigla abreviada utilizada em relatórios e telas compactas.
        /// </summary>
        /// <example>kWh, L, m³.</example>
        [Column("abreviatura")]
        public string Abreviatura { get; set; }

        /// <summary>
        /// Indica a vigência da unidade de medida no sistema.
        /// </summary>
        /// <remarks>
        /// Valores controlados pelo Enum <see cref="Situacao"/>:
        /// 1 - ATIVO: Unidade disponível para seleção em novos cadastros.
        /// 2 - INATIVO: Unidade obsoleta (não deve ser listada para novos registros).
        /// </remarks>
        [Column("situacao")]
        public Situacao Situacao { get; set; }

        /// <summary>
        /// Relacionamento: Coleção de Tipos de Despesa que utilizam esta unidade de medida específica.
        /// </summary>
        public ICollection<TipoDespesa> TiposDespesas { get; set; }

        /// <summary>
        /// Construtor para inicialização completa da entidade UnidadeMedida.
        /// </summary>

        [Column("excluido")]
        public bool Excluido { get; set; }

        [Column("dataexclusao")]
        public DateTime? DataExclusao { get; set; }

        public UnidadeMedida(int id, string descricao, string abreviatura, Situacao situacao)
        {
            Id = id;
            Descricao = descricao;
            Abreviatura = abreviatura;
            Situacao = situacao;
        }
    }
}
