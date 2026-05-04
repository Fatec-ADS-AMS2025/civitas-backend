namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    /// <summary>
    /// Objeto de transfer�ncia para gest�o dos tetos or�ament�rios (Previs�o de Gastos).
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Input: Cadastrar ou ajustar o limite de gastos de uma institui��o para um determinado ano e tipo de despesa.
    /// - Output: Exibir o planejamento financeiro aprovado.
    /// </remarks>
    public class OrcamentoDTO
    {
        /// <summary>
        /// Identificador �nico do or�amento.
        /// </summary>
        /// <remarks>
        /// Input: Ignorado na cria��o. Obrigat�rio na edi��o.
        /// </remarks>
        public int IdOrcamento { get; set; }

        /// <summary>
        /// Ano fiscal de refer�ncia (Exerc�cio).
        /// </summary>
        /// <example>2024, 2025.</example>
        /// <remarks>
        /// Valida��o: Deve ser um ano v�lido (4 d�gitos). Geralmente n�o se permite cadastrar or�amentos para anos passados.
        /// </remarks>
        public int AnoOrcamento { get; set; }

        /// <summary>
        /// Valor monet�rio total dispon�vel (Teto).
        /// </summary>
        /// <remarks>
        /// Regra: Deve ser maior que zero.
        /// Este valor ser� o limite usado para validar se as despesas lan�adas estouraram o or�amento.
        /// </remarks>
        public decimal ValorOrcamento { get; set; }

        /// <summary>
        /// Identificador da Institui��o dona deste or�amento.
        /// </summary>
        /// <remarks>
        /// Obrigat�rio. Define qual unidade (escola, posto, secretaria) poder� usar este recurso.
        /// </remarks>
        public int IdInstituicao { get; set; }
        public int IdTipoDespesa { get; set; }
    }
}