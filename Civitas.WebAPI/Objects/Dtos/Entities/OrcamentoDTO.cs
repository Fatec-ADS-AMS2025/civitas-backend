namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    /// <summary>
    /// Objeto de transferência para gestão dos tetos orçamentários (Previsão de Gastos).
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Input: Cadastrar ou ajustar o limite de gastos de uma instituição para um determinado ano e tipo de despesa.
    /// - Output: Exibir o planejamento financeiro aprovado.
    /// </remarks>
    public class OrcamentoDTO
    {
        /// <summary>
        /// Identificador único do orçamento.
        /// </summary>
        /// <remarks>
        /// Input: Ignorado na criação. Obrigatório na edição.
        /// </remarks>
        public int IdOrcamento { get; set; }

        /// <summary>
        /// Ano fiscal de referência (Exercício).
        /// </summary>
        /// <example>2024, 2025.</example>
        /// <remarks>
        /// Validação: Deve ser um ano válido (4 dígitos). Geralmente não se permite cadastrar orçamentos para anos passados.
        /// </remarks>
        public int AnoOrcamento { get; set; }

        /// <summary>
        /// Valor monetário total disponível (Teto).
        /// </summary>
        /// <remarks>
        /// Regra: Deve ser maior que zero.
        /// Este valor será o limite usado para validar se as despesas lançadas estouraram o orçamento.
        /// </remarks>
        public double ValorOrcamento { get; set; }

        /// <summary>
        /// Identificador da Instituição dona deste orçamento.
        /// </summary>
        /// <remarks>
        /// Obrigatório. Define qual unidade (escola, posto, secretaria) poderá usar este recurso.
        /// </remarks>
        public int IdInstituicao { get; set; }

        /// <summary>
        /// Identificador do Tipo de Despesa coberto por este orçamento.
        /// </summary>
        /// <remarks>
        /// Obrigatório. O orçamento é segregado por categoria (ex: "Verba apenas para Energia Elétrica").
        /// </remarks>
        public int IdTipoDespesa { get; set; }
    }
}