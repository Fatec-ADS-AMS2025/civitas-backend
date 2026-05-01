using Civitas.WebAPI.Objects.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Models
{
    /// <summary>
    /// Entidade mínima que representa uma unidade consumidora vinculada a uma despesa.
    /// </summary>
    [Table("unidadeconsumidora")]
    public class UnidadeConsumidora
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("codigo")]
        public string Codigo { get; set; }

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

        public ICollection<Despesa> Despesas { get; set; }

        public UnidadeConsumidora()
        {
        }

        public UnidadeConsumidora(
            int id,
            string codigo,
            Situacao situacao,
            int idTipoDespesa,
            int idOrcamento,
            int idInstituicao,
            int idFornecedor)
        {
            Id = id;
            Codigo = codigo;
            Situacao = situacao;
            IdTipoDespesa = idTipoDespesa;
            IdOrcamento = idOrcamento;
            IdInstituicao = idInstituicao;
            IdFornecedor = idFornecedor;
        }
    }
}
