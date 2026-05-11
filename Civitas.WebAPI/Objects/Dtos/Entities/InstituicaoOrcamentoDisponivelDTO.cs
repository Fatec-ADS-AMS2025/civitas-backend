namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    public class InstituicaoOrcamentoDisponivelDTO
    {
        public int IdInstituicao { get; set; }
        public string NomeInstituicao { get; set; }
        public decimal TotalOrcamentoDisponivel { get; set; }

        public decimal JaneiroOrcamentoDisponivel { get; set; }
        public decimal FevereiroOrcamentoDisponivel { get; set; }
        public decimal MarcoOrcamentoDisponivel { get; set; }
        public decimal AbrilOrcamentoDisponivel { get; set; }
        public decimal MaioOrcamentoDisponivel { get; set; }
        public decimal JunhoOrcamentoDisponivel { get; set; }
        public decimal JulhoOrcamentoDisponivel { get; set; }
        public decimal AgostoOrcamentoDisponivel { get; set; }
        public decimal SetembroOrcamentoDisponivel { get; set; }
        public decimal OutubroOrcamentoDisponivel { get; set; }
        public decimal NovembroOrcamentoDisponivel { get; set; }
        public decimal DezembroOrcamentoDisponivel { get; set; }
    }
}
