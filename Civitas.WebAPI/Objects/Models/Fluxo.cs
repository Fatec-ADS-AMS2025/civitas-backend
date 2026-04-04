using Civitas.WebAPI.Objects.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Models
{
    /// <summary>
    /// Representa um registro financeiro específico de pagamento ou medição (uma parcela ou competência).
    /// Mapeia a tabela 'fluxo' do banco de dados.
    /// </summary>
    /// <remarks>
    /// O Fluxo armazena os dados reais de execução: quanto foi consumido e quanto foi efetivamente pago em um determinado período.
    /// É aqui que o controle de inadimplência ocorre através do campo <see cref="Status"/>.
    /// </remarks>
    [Table("fluxo")]
    public class Fluxo
    {
        /// <summary>
        /// Identificador único do fluxo financeiro (Chave Primária).
        /// </summary>
        [Column("idfluxo")]
        public int IdFluxo { get; set; }

        /// <summary>
        /// Valor monetário efetivamente pago.
        /// </summary>
        /// <remarks>
        /// Este valor pode diferir do valor orçado. Deve ser preenchido no momento da quitação.
        /// </summary>
        [Column("valorpago")]
        public float ValorPago { get; set; }

        /// <summary>
        /// Quantidade consumida referente a este fluxo.
        /// </summary>
        /// <remarks>
        /// A unidade de medida deste valor depende da configuração da Despesa vinculada (ex: kWh, m³, Unidades).
        /// Importante para relatórios de eficiência e sustentabilidade.
        /// </remarks>
        [Column("consumo")]
        public int Consumo { get; set; }

        /// <summary>
        /// Estado atual do pagamento deste fluxo.
        /// </summary>
        /// <remarks>
        /// Controlado pelo Enum <see cref="Status"/>:
        /// 1 - A_PAGAR: Aguardando quitação.
        /// 2 - PAGA: Processo finalizado com sucesso.
        /// 3 - ATRASADO: Data de vencimento expirada sem pagamento.
        /// </remarks>
        [Column("status")]
        public Status Status { get; set; }

        /// <summary>
        /// Coleção de comprovantes, notas fiscais ou boletos anexados a este pagamento específico.
        /// </summary>
        public ICollection<Documento> Documentos { get; set; }

        /// <summary>
        /// Construtor vazio para inicialização pelo Entity Framework.
        /// </summary>
        public Fluxo()
        {
        }

        /// <summary>
        /// Construtor para inicialização completa da entidade Fluxo.
        /// </summary>
        public Fluxo(int idfluxo, float valorpago, int consumo, Status status)
        {
            IdFluxo = idfluxo;
            ValorPago = valorpago;
            Consumo = consumo;
            Status = status;
        }
    }
}