using Civitas.WebAPI.Objects.Enums;

namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    public class FluxoDTO
    {
        public int IdFluxo { get; set; }
        public string ValorPago { get; set; }
        public string Consumo { get; set; }
        public Status Status { get; set; }
    }
}
