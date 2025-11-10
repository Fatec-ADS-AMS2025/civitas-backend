using Civitas.WebAPI.Objects.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Models
{
    [Table("fluxo")]
    public class Fluxo
    {
        [Column("idfluxo")]
        public int IdFluxo { get; set; }

        [Column("valorpago")]
        public float ValorPago { get; set; }

        [Column("consumo")]
        public int Consumo { get; set; }

        [Column("status")]
        public Status Status { get; set; }

        public Fluxo()
        {
        }

        public Fluxo(int idfluxo, float valorpago, int consumo, Status status)
        {
            IdFluxo = idfluxo;
            ValorPago = valorpago;
            Consumo = consumo;
            Status = status;
        }
    }
}
