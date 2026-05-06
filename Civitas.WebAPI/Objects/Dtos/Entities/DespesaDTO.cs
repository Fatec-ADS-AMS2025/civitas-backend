using System.Text.Json;
using Civitas.WebAPI.Objects.Enums;

namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    /// <summary>
    /// Objeto de transferência principal para lançamento e gestão de despesas.
    /// </summary>
    public class DespesaDTO
    {
        /// <summary>
        /// Identificador único da despesa.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Número do documento fiscal ou identificador da fatura.
        /// </summary>
        public string NumeroDocumento { get; set; }

        /// <summary>
        /// Código identificador da despesa.
        /// </summary>
        public string Codigo { get; set; }

        /// <summary>
        /// Data de emissão do documento.
        /// </summary>
        public DateOnly DataEmissao { get; set; }

        /// <summary>
        /// Valor previsto para a despesa.
        /// </summary>
        public decimal ValorPrevisto { get; set; }

        /// <summary>
        /// Valor realmente pago.
        /// </summary>
        public decimal ValorPago { get; set; }

        /// <summary>
        /// Previsão de consumo para esta despesa.
        /// </summary>
        public decimal ConsumoPrevisto { get; set; }

        /// <summary>
        /// Consumo real registrado para esta despesa.
        /// </summary>
        public decimal ConsumoReal { get; set; }

        /// <summary>
        /// Data de vencimento da fatura.
        /// </summary>
        public DateOnly DataVencimento { get; set; }

        /// <summary>
        /// Status financeiro da despesa.
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// Identificador do Usuário responsável pelo lançamento.
        /// </summary>
        public int IdUsuario { get; set; }

        /// <summary>
        /// Identificador da unidade consumidora vinculada.
        /// </summary>
        public int IdUnidadeConsumidora { get; set; }

        /// <summary>
        /// Valores preenchidos para os campos opcionais. Apenas chaves declaradas
        /// no TipoDespesa relacionado são aceitas. Pode ser null, vazio, ou parcial.
        /// </summary>
        /// <example>{"numeroNota":"12345","fornecedor":"Papelaria Central","centroCusto":null}</example>
        public IDictionary<string, JsonElement>? ValoresOpcionais { get; set; }
    }
}
