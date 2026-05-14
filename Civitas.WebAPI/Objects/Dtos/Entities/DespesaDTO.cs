using System.Text.Json;
using Civitas.WebAPI.Objects.Enums;

namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    /// <summary>
    /// Objeto de transferÃªncia principal para lanÃ§amento e gestÃ£o de despesas.
    /// </summary>
    public class DespesaDTO
    {
        /// <summary>
        /// Identificador Ãºnico da despesa.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// NÃºmero do documento fiscal ou identificador da fatura.
        /// </summary>
        public string NumeroDocumento { get; set; }

        public string? NomeDocumento { get; set; }

        public string? HashDocumento { get; set; }

        /// <summary>
        /// CÃ³digo identificador da despesa.
        /// </summary>
        public string Codigo { get; set; }

        /// <summary>
        /// Data de emissÃ£o do documento.
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

        public decimal Juros { get; set; }

        public decimal Multa { get; set; }

        public decimal Desconto { get; set; }

        /// <summary>
        /// PrevisÃ£o de consumo para esta despesa.
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

        public DateOnly? DataPagamento { get; set; }

        /// <summary>
        /// Status financeiro da despesa.
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// Identificador do UsuÃ¡rio responsÃ¡vel pelo lanÃ§amento.
        /// </summary>
        public int IdUsuario { get; set; }

        /// <summary>
        /// Identificador da unidade consumidora vinculada.
        /// </summary>
        public int IdUnidadeConsumidora { get; set; }

        /// <summary>
        /// Valores preenchidos para os campos opcionais. Apenas chaves declaradas
        /// no TipoDespesa relacionado sÃ£o aceitas. Pode ser null, vazio, ou parcial.
        /// </summary>
        /// <example>{"numeroNota":"12345","fornecedor":"Papelaria Central","centroCusto":null}</example>
        public IDictionary<string, JsonElement>? ValoresOpcionais { get; set; }
        public bool Excluido { get; set; }

        public DateTime? DataExclusao { get; set; }

    }
}


