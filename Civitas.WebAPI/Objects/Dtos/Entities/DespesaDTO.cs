using Civitas.WebAPI.Objects.Enums;

namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    /// <summary>
    /// Objeto de transferência principal para lançamento e gestão de despesas (Contas a Pagar).
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Input: Formulário principal de cadastro de despesas.
    /// - Output: Detalhamento da despesa para consulta.
    /// </remarks>
    public class DespesaDTO
    {
        /// <summary>
        /// Identificador único da despesa.
        /// </summary>
        /// <remarks>
        /// Input: Nulo/Zero na criação. Obrigatório na edição.
        /// </remarks>
        public int Id { get; set; }

        /// <summary>
        /// Número do documento fiscal ou identificador da fatura.
        /// </summary>
        /// <remarks>
        /// Obrigatório. Usado para evitar lançamentos duplicados da mesma conta.
        /// </remarks>
        public string NumeroDocumento { get; set; }

        public int Codigo { get; set; }

        /// <summary>
        /// Código da Unidade Consumidora (UC).
        /// </summary>
        /// <remarks>
        /// Validação Condicional: A obrigatoriedade deste campo depende do <see cref="IdTipoDespesa"/> selecionado.
        /// Se o tipo de despesa exigir UC (ex: Energia), este campo não pode ser nulo.
        /// </remarks>
        public string UC { get; set; }

        /// <summary>
        /// Data de emissão do documento.
        /// </summary>
        /// <remarks>
        /// Formato: String. Recomenda-se o padrão ISO 8601 (AAAA-MM-DD) para facilitar a ordenação e filtros por data.
        /// </remarks>
        public string DataEmissao { get; set; }

        /// <summary>
        /// Previsão de consumo para esta fatura.
        /// </summary>
        /// <remarks>
        /// Unidade de medida implícita baseada no <see cref="IdTipoDespesa"/>.
        /// </remarks>
        public double ConsumoPrevisto { get; set; }

        /// <summary>
        /// Data de vencimento da fatura.
        /// </summary>
        /// <remarks>
        /// Obrigatório. Define o prazo limite para pagamento.
        /// </remarks>
        public DateOnly DataVencimento { get; set; }

        /// <summary>
        /// Status financeiro da despesa.
        /// </summary>
        /// <remarks>
        /// Valores: <see cref="Status"/> (A_PAGAR, PAGA, ATRASADO).
        /// </remarks>
        public Status Status { get; set; }

        /// <summary>
        /// Identificador da Categoria da Despesa.
        /// </summary>
        /// <remarks>
        /// Obrigatório. Define as regras de validação (se pede UC, qual a unidade de medida).
        /// </remarks>
        public int IdTipoDespesa { get; set; }

        /// <summary>
        /// Identificador do Orçamento vinculado.
        /// </summary>
        /// <remarks>
        /// Obrigatório. O sistema deve validar se o orçamento possui saldo suficiente para esta despesa.
        /// </remarks>
        public int IdOrcamento { get; set; }

        /// <summary>
        /// Identificador da Instituição pagadora.
        /// </summary>
        public int IdInstituicao { get; set; }

        /// <summary>
        /// Identificador do Fornecedor (Credor).
        /// </summary>
        public int IdFornecedor { get; set; }

        /// <summary>
        /// Identificador do Usuário responsável pelo lançamento.
        /// </summary>
        /// <remarks>
        /// Em operações de criação (POST), este ID geralmente é extraído automaticamente do Token de autenticação do usuário logado, 
        /// mas pode ser enviado explicitamente se o sistema permitir lançar em nome de terceiros.
        /// </remarks>
        public int IdUsuario { get; set; }
    }
}