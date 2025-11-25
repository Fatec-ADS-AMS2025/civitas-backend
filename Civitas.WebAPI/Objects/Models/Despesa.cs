using Civitas.WebAPI.Objects.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Models
{
    [Table("despesa")]
    public class Despesa
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("numerodocumento")]
        public string NumeroDocumento { get; set; }
        [Column("uc")]
        public string UC { get; set; }
        [Column("dataemissao")]
        public string DataEmicao { get; set; }
        [Column("consumoprevisto")]
        public double ConsumoPrevisto { get; set; }
        [Column("datavencimento")]
        public DateOnly DataVencimento { get; set; }

        [Column("situacao")]
        public Situacao Situacao { get; set; }

        [Column("idtipodespesa")]
        public int IdTipoDespesa { get; set; }
        public TipoDespesa TipoDespesa { get; set; }

        [Column("idorcamento")]
        public int IdOrcamento { get; set; }
        public Orcamento Orcamento { get; set; }

        [Column("idinstituicao")]
        public int IdInstituicao { get; set; }
        public Instituicao Instituicao { get; set; }

        [Column("idfornecedor")]
        public int IdFornecedor { get; set; }
        public Fornecedor Fornecedor { get; set; }

        [Column("idusuario")]
        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; }

        public Despesa()
        {
            
        }

        public Despesa(int id, string numeroDocumento, string uc, string dataEmicao, double consumoPrevisto, DateOnly dataVencimento, Situacao situacao)
        {
            Id = id;
            NumeroDocumento = numeroDocumento;
            UC = uc;
            DataEmicao = dataEmicao;
            ConsumoPrevisto = consumoPrevisto;
            DataVencimento = dataVencimento;
            Situacao = situacao;
        }
    }
}
