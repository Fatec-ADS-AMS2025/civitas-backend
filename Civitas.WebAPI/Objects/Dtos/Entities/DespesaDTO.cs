using Civitas.WebAPI.Objects.Enums;

namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    public class DespesaDTO
    {
        public int Id { get; set; }
        public string NumeroDocumento { get; set; }
        public string UC { get; set; }
        public string DataEmicao { get; set; }
        public double ConsumoPrevisto { get; set; }
        public DateOnly DataVencimento { get; set; }
        public Situacao Situacao { get; set; }
        public int IdTipoDespesa { get; set; }
        public int IdOrcamento { get; set; }
        public int IdInstituicao { get; set; }
        public int IdFornecedor { get; set; }
        public int IdUsuario { get; set; }

    }
}
