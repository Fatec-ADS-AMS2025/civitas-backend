using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    /// <summary>
    /// Objeto de transferência para as categorias de despesas (Configurações de Lançamento).
    /// </summary>
    /// <remarks>
    /// Este DTO é fundamental para a UI (Interface do Usuário), pois dita as regras do formulário de lançamento de despesas:
    /// qual unidade de medida exibir (kWh, m³) e se o campo "UC" deve ser obrigatório ou oculto.
    /// </remarks>
    public class TipoDespesaDTO
    {
        /// <summary>
        /// Identificador único do tipo de despesa.
        /// </summary>
        /// <remarks>
        /// Input: Ignorado na criação (POST).
        /// Output: Identificador gerado (GET).
        /// </remarks>
        public int Id { get; set; }

        /// <summary>
        /// Descrição textual da categoria.
        /// </summary>
        /// <example>Energia Elétrica, Água e Esgoto, Internet.</example>
        public string Descricao { get; set; }

        /// <summary>
        /// Define a obrigatoriedade do campo Unidade Consumidora (UC).
        /// </summary>
        /// <remarks>
        /// Valor baseado no Enum <see cref="SolicitaUc"/>.
        /// Se valor = 1 (Sim), o front-end deve renderizar o input de UC como obrigatório.
        /// </remarks>
        public SolicitaUc SolicitaUc { get; set; }

        /// <summary>
        /// Estado de ativação do tipo de despesa.
        /// </summary>
        /// <remarks>
        /// Valores: <see cref="Situacao"/>.
        /// </remarks>
        public Situacao Situacao { get; set; }

        /// <summary>
        /// Referência à Unidade de Medida padrão para este tipo de gasto.
        /// </summary>
        /// <remarks>
        /// O front-end utiliza este ID para buscar a sigla correta (ex: kWh) e exibi-la ao lado do campo de "Consumo".
        /// </remarks>
        public int IdUnidadeMedida { get; set; }
        public int IdTipoCodigo { get; set; }

        /// <summary>
        /// Lista de nomes de campos opcionais aceitos pelas despesas deste tipo.
        /// Pode ser null ou vazia. Nomes únicos (case-insensitive), até 100 caracteres cada,
        /// no máximo 50 itens.
        /// </summary>
        /// <example>["numeroNota","fornecedor","centroCusto"]</example>
        public IList<string>? CamposOpcionais { get; set; }
    }
}