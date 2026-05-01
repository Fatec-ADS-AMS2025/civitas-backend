namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    public class SecretariaGastosDTO
    {
        public int IdSecretaria { get; set; }
        public string NomeSecretaria { get; set; }
        public int QuantidadeInstituicoes { get; set; }
        public int QuantidadeDespesas { get; set; }
        public decimal TotalGastos { get; set; }
    }
}
