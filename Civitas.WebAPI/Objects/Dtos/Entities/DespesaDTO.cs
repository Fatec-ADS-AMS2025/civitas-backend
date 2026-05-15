using Civitas.WebAPI.Objects.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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
        /// Nome do documento fornecido pelo usuário
        /// </summary>
        public string? NomeDocumento { get; set; }

        /// <summary>
        /// Hash SHA-256 do documento (gerado automaticamente pelo código)
        /// </summary>
        public string? HashDocumento { get; set; }

        /// <summary>
        /// Campo para confirmação de documento duplicado.
        /// </summary>
        public Boolean ConfirmarDocumentoDuplicado { get; set; }

        /// <summary>
        /// Campo de arquivo de Documento para upload.
        /// </summary>
        public IFormFile? Documento { get; set; }

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

        public DateOnly? DataPagamento { get; set; }

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
        public string? ValoresOpcionais { get; set; }
    }
}
