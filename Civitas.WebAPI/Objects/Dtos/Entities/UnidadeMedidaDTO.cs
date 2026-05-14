using Civitas.WebAPI.Objects.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    /// <summary>
    /// DTO leve utilizado para transporte de dados de Unidades de Medida.
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Alimentar listas de seleção (dropdowns) no front-end.
    /// - Transportar dados para cadastro e edição de novas unidades.
    /// </remarks>
    public class UnidadeMedidaDTO
    {
        /// <summary>
        /// Identificador da unidade.
        /// </summary>
        /// <remarks>
        /// Entrada: Opcional/Ignorado na criação. Obrigatório na edição.
        /// </remarks>
        public int Id { get; set; }

        /// <summary>
        /// Nome descritivo da unidade.
        /// </summary>
        /// <remarks>
        /// Obrigatório. Ex: "Quilowatts-hora", "Metros Cúbicos".
        /// </remarks>
        public string Descricao { get; set; }

        /// <summary>
        /// Sigla curta para exibição em tabelas ou relatórios.
        /// </summary>
        /// <remarks>
        /// Obrigatório. Ex: "kWh", "m³".
        /// </remarks>
        public string Abreviatura { get; set; }

        /// <summary>
        /// Status da unidade no sistema.
        /// </summary>
        /// <remarks>
        /// Valores controlados pelo Enum <see cref="Civitas.WebAPI.Objects.Enums.Situacao"/>.
        /// Unidades "Inativas" não devem ser exibidas em listas de seleção para novos cadastros.
        /// </remarks>
        public Situacao Situacao { get; set; }
        public bool Excluido { get; set; }

        public DateTime? DataExclusao { get; set; }

    }
}
