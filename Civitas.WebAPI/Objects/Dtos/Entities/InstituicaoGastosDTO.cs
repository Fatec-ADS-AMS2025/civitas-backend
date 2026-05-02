namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    public class InstituicaoGastosDTO
    {
        public int IdInstituicao { get; set; }
        public string NomeInstituicao { get; set; }
        public int QuantidadeDespesas { get; set; }
        public double TotalGastos { get; set; }
    }
}
