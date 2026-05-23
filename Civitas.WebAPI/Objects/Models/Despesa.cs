using Civitas.WebAPI.Objects.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Models
{
    /// <summary>
    /// Entidade central que representa uma obrigaÃ§Ã£o financeira, conta ou fatura a ser paga.
    /// Mapeia a tabela 'despesa' do banco de dados.
    /// </summary>
    [Table("despesa")]
    public class Despesa : ISoftDeletable
    {
        /// <summary>
        /// Identificador Ãºnico da despesa (Chave PrimÃ¡ria).
        /// </summary>
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// NÃºmero de identificaÃ§Ã£o impresso no documento fiscal (NF, Fatura, Boleto).
        /// </summary>
        [Column("numerodocumento")]
        public string NumeroDocumento { get; set; }

        /// <summary>
        /// Nome de identificação do documento
        /// </summary>
        [Column("nomedocumento")]
        public string? NomeDocumento { get; set; }


        /// <summary>
        /// Hash de identificação do documento
        /// </summary>
        [Column("hashdocumento")]
        public string? HashDocumento { get; set; }

        /// <summary>
        /// Código identificador da despesa.
        /// </summary>
        [Column("codigo")]
        public string Codigo { get; set; }

        /// <summary>
        /// Data em que o documento foi emitido.
        /// </summary>
        [Column("dataemissao")]
        public DateOnly DataEmissao { get; set; }

        /// <summary>
        /// Valor previsto para a despesa.
        /// </summary>
        [Column("valorprevisto")]
        public decimal ValorPrevisto { get; set; }

        /// <summary>
        /// Valor realmente pago.
        /// </summary>
        [Column("valorpago")]
        public decimal ValorPago { get; set; }

        [Column("juros")]
        public decimal Juros { get; set; }

        [Column("multa")]
        public decimal Multa { get; set; }

        [Column("desconto")]
        public decimal Desconto { get; set; }

        /// <summary>
        /// Consumo previsto para esta despesa.
        /// </summary>
        [Column("consumoprevisto")]
        public decimal ConsumoPrevisto { get; set; }

        /// <summary>
        /// Consumo real registrado para esta despesa.
        /// </summary>
        [Column("consumoreal")]
        public decimal ConsumoReal { get; set; }

        /// <summary>
        /// Data limite para o pagamento sem incidÃªncia de juros ou multas.
        /// </summary>
        [Column("datavencimento")]
        public DateOnly DataVencimento { get; set; }

        [Column("datapagamento")]
        public DateOnly? DataPagamento { get; set; }

        /// <summary>
        /// Status financeiro da despesa.
        /// </summary>
        [Column("status")]
        public Status Status { get; set; }

        /// <summary>
        /// Chave estrangeira do UsuÃ¡rio que cadastrou esta despesa.
        /// </summary>
        [Column("idusuario")]
        public int IdUsuario { get; set; }

        /// <summary>
        /// Objeto de navegaÃ§Ã£o do UsuÃ¡rio.
        /// </summary>
        public Usuario Usuario { get; set; }

        /// <summary>
        /// Chave estrangeira da unidade consumidora vinculada.
        /// </summary>
        [Column("idunidadeconsumidora")]
        public int IdUnidadeConsumidora { get; set; }

        /// <summary>
        /// Objeto de navegaÃ§Ã£o da unidade consumidora.
        /// </summary>
        public UnidadeConsumidora UnidadeConsumidora { get; set; }

        /// <summary>
        /// Valores dos campos opcionais preenchidos para esta despesa.
        /// Formato esperado: objeto JSON plano, ex: {"nomeCampo1":"valor","nomeCampo2":null}.
        /// Apenas chaves declaradas em TipoDespesa.CamposOpcionais sÃ£o aceitas.
        /// </summary>
        [Column("valoresopcionais")]
        public string? ValoresOpcionais { get; set; }


        [Column("excluido")]
        public bool Excluido { get; set; }

        [Column("dataexclusao")]
        public DateTime? DataExclusao { get; set; }

        public Despesa()
        {
        }

        public Despesa(
            int id,
            string numeroDocumento,
            string? nomeDocumento,
            string? hashDocumento,
            string codigo,
            DateOnly dataEmissao,
            decimal valorPrevisto,
            decimal valorPago,
            decimal consumoPrevisto,
            decimal consumoReal,
            DateOnly dataVencimento,
            DateOnly? dataPagamento,
            Status status,
            int idUsuario,
            Usuario usuario,
            int idUnidadeConsumidora,
            UnidadeConsumidora unidadeConsumidora,
            string? valoresOpcionais)
        {
            Id = id;
            NumeroDocumento = numeroDocumento;
            NomeDocumento = nomeDocumento;
            HashDocumento = hashDocumento;
            Codigo = codigo;
            DataEmissao = dataEmissao;
            ValorPrevisto = valorPrevisto;
            ValorPago = valorPago;
            Juros = 0;
            Multa = 0;
            Desconto = 0;
            ConsumoPrevisto = consumoPrevisto;
            ConsumoReal = consumoReal;
            DataVencimento = dataVencimento;
            DataPagamento = dataPagamento;
            Status = status;
            IdUsuario = idUsuario;
            Usuario = usuario;
            IdUnidadeConsumidora = idUnidadeConsumidora;
            UnidadeConsumidora = unidadeConsumidora;
            ValoresOpcionais = valoresOpcionais;
        }
    }
}


