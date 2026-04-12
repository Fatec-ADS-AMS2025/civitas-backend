namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    public class SecretariaOrcamentoDisponivelDTO
    {
        public int IdSecretaria { get; set; }
        public string NomeSecretaria { get; set; }
        public int QuantidadeInstituicoes { get; set; }
        public decimal TotalOrcamentoDisponivel { get; set; }
    }
}
