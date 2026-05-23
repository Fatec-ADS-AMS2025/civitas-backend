namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    /// <summary>
    /// Objeto de transferï¿½ncia para gestï¿½o dos tetos orï¿½amentï¿½rios (Previsï¿½o de Gastos).
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Input: Cadastrar ou ajustar o limite de gastos de uma instituiï¿½ï¿½o para um determinado ano e tipo de despesa.
    /// - Output: Exibir o planejamento financeiro aprovado.
    /// </remarks>
    public class OrcamentoDTO
    {
        /// <summary>
        /// Identificador ï¿½nico do orï¿½amento.
        /// </summary>
        /// <remarks>
        /// Input: Ignorado na criaï¿½ï¿½o. Obrigatï¿½rio na ediï¿½ï¿½o.
        /// </remarks>
        public int IdOrcamento { get; set; }

        /// <summary>
        /// Ano fiscal de referï¿½ncia (Exercï¿½cio).
        /// </summary>
        /// <example>2024, 2025.</example>
        /// <remarks>
        /// Validaï¿½ï¿½o: Deve ser um ano vï¿½lido (4 dï¿½gitos). Geralmente nï¿½o se permite cadastrar orï¿½amentos para anos passados.
        /// </remarks>
        public int AnoOrcamento { get; set; }

        /// <summary>
        /// Valor monetï¿½rio total disponï¿½vel (Teto).
        /// </summary>
        /// <remarks>
        /// Regra: Deve ser maior que zero.
        /// Este valor serï¿½ o limite usado para validar se as despesas lanï¿½adas estouraram o orï¿½amento.
        /// </remarks>
        public decimal? ValorOrcamento { get; set; }

        public decimal JaneiroQuantidadeConsumo { get; set; }
        public decimal JaneiroValorConsumo { get; set; }

        public decimal FevereiroQuantidadeConsumo { get; set; }
        public decimal FevereiroValorConsumo { get; set; }

        public decimal MarcoQuantidadeConsumo { get; set; }
        public decimal MarcoValorConsumo { get; set; }

        public decimal AbrilQuantidadeConsumo { get; set; }
        public decimal AbrilValorConsumo { get; set; }

        public decimal MaioQuantidadeConsumo { get; set; }
        public decimal MaioValorConsumo { get; set; }

        public decimal JunhoQuantidadeConsumo { get; set; }
        public decimal JunhoValorConsumo { get; set; }

        public decimal JulhoQuantidadeConsumo { get; set; }
        public decimal JulhoValorConsumo { get; set; }

        public decimal AgostoQuantidadeConsumo { get; set; }
        public decimal AgostoValorConsumo { get; set; }

        public decimal SetembroQuantidadeConsumo { get; set; }
        public decimal SetembroValorConsumo { get; set; }

        public decimal OutubroQuantidadeConsumo { get; set; }
        public decimal OutubroValorConsumo { get; set; }

        public decimal NovembroQuantidadeConsumo { get; set; }
        public decimal NovembroValorConsumo { get; set; }

        public decimal DezembroQuantidadeConsumo { get; set; }
        public decimal DezembroValorConsumo { get; set; }

        /// <summary>
        /// Identificador da Instituiï¿½ï¿½o dona deste orï¿½amento.
        /// </summary>
        /// <remarks>
        /// Obrigatï¿½rio. Define qual unidade (escola, posto, secretaria) poderï¿½ usar este recurso.
        /// </remarks>
        public int IdInstituicao { get; set; }
        public int IdTipoDespesa { get; set; }
        public bool Excluido { get; set; }

        public DateTime? DataExclusao { get; set; }

    }
}
