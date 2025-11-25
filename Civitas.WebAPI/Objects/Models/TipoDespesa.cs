using Civitas.WebAPI.Objects.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Models
{
    [Table("tipodespesa")]
    public class TipoDespesa
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("descricao")]
        public string Descricao { get; set; }

        [Column("idunidademedida")]
        public int IdUnidadeMedida { get; set; }
        public UnidadeMedida UnidadeMedida { get; set; }

        [Column("solicitauc")]
        public SolicitaUc SolicitaUc { get; set; }

        [Column("situacao")]
        public Situacao Situacao { get; set; }

        public ICollection<Despesa> Despesas { get; set; }

        public TipoDespesa(int id, string descricao, SolicitaUc solicitaUc, Situacao situacao)
        {
            Id = id;
            Descricao = descricao;
            SolicitaUc = solicitaUc;
            Situacao = situacao;
        }
    }
}
