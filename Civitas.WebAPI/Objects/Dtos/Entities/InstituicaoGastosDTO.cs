namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    public class InstituicaoGastosDTO
    {
        public int IdInstituicao { get; set; }
        public string NomeInstituicao { get; set; }
        public int QuantidadeDespesas { get; set; }
        public decimal TotalGastos { get; set; }

        public decimal JaneiroGastos { get; set; }
        public decimal FevereiroGastos { get; set; }
        public decimal MarcoGastos { get; set; }
        public decimal AbrilGastos { get; set; }
        public decimal MaioGastos { get; set; }
        public decimal JunhoGastos { get; set; }
        public decimal JulhoGastos { get; set; }
        public decimal AgostoGastos { get; set; }
        public decimal SetembroGastos { get; set; }
        public decimal OutubroGastos { get; set; }
        public decimal NovembroGastos { get; set; }
        public decimal DezembroGastos { get; set; }
    }
}
