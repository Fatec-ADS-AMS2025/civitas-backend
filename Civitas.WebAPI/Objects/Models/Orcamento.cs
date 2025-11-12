using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Models
{
    [Table("orcamento")]
    public class Orcamento
    {
        [Column("idorcamento")]
        public int IdOrcamento { get; set; }

        [Column("anoorcamento")]
        public int AnoOrcamento { get; set; }

        [Column("valororcamento")]
        public double ValorOrcamento { get; set; }

        [Column("idinstituicao")]
        public int IdInstituicao { get; set; }
        public Instituicao? Instituicao { get; set; }

        [Column("idtipodespesa")]
        public int IdTipoDespesa { get; set; }
        public TipoDespesa? TipoDespesa { get; set; }

        public ICollection<Despesa> Despesas { get; set; }

        public Orcamento()
        {
            
        }

        public Orcamento(int idOrcamento, int anoOrcamento, double valorOrcamento, int idInstituicao)
        {
            IdOrcamento = idOrcamento;
            AnoOrcamento = anoOrcamento;
            ValorOrcamento = valorOrcamento;
            IdInstituicao = idInstituicao;
        }
    }
}
