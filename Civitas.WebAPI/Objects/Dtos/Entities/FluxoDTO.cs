using Civitas.WebAPI.Objects.Enums;

namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    /// <summary>
    /// Objeto de transferência para atualização de dados de execução financeira (Pagamentos e Medições).
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Input: Registrar a baixa de um pagamento (informar valor pago e consumo real).
    /// - Output: Exibir o histórico de pagamentos de uma despesa.
    /// </remarks>
    public class FluxoDTO
    {
        /// <summary>
        /// Identificador único do fluxo (parcela/competência).
        /// </summary>
        /// <remarks>
        /// Input: Obrigatório para identificar qual registro está sendo atualizado (PUT/PATCH).
        /// </remarks>
        public int IdFluxo { get; set; }

        /// <summary>
        /// Valor monetário efetivamente pago.
        /// </summary>
        /// <remarks>
        /// Pode divergir do valor original da despesa em caso de multas, juros ou descontos.
        /// </remarks>
        public float ValorPago { get; set; }

        /// <summary>
        /// Medição do consumo no período.
        /// </summary>
        /// <remarks>
        /// Ex: Quantidade de kWh ou m³ gastos.
        /// Essencial para relatórios de eficiência energética/hídrica.
        /// </remarks>
        public int Consumo { get; set; }

        /// <summary>
        /// Situação atual do pagamento.
        /// </summary>
        /// <remarks>
        /// Valores: <see cref="Status"/> (1-A Pagar, 2-Paga, 3-Atrasado).
        /// Ao alterar para "Paga" (2), o sistema geralmente exige que o campo ValorPago > 0.
        /// </remarks>
        public Status Status { get; set; }
    }
}