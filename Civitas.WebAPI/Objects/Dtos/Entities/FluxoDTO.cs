using Civitas.WebAPI.Objects.Enums;

namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    public class FluxoDTO
    {
        public int IdFluxo { get; set; }
        public float ValorPago { get; set; }
        public int Consumo { get; set; }
        public Status Status { get; set; }

    }
}
