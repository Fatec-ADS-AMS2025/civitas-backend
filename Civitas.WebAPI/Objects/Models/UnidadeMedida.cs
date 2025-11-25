using Civitas.WebAPI.Objects.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Models
{
    [Table("unidademedida")]
    public class UnidadeMedida
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("descricao")]
        public string Descricao { get; set; }
        [Column("abreviatura")]
        public string Abreviatura { get; set; }

        [Column("situacao")]
        public Situacao Situacao { get; set; }

        public ICollection<TipoDespesa> TiposDespesas { get; set; }


        public UnidadeMedida(int id, string descricao, string abreviatura, Situacao situacao)
        {
            Id = id;
            Descricao = descricao;
            Abreviatura = abreviatura;
            Situacao = situacao;
        }
    }
}
