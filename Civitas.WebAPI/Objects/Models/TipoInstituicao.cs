using Civitas.WebAPI.Objects.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Models
{
    [Table("tipoinstituicao")]
    public class TipoInstituicao
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("descricao")]
        public string Descricao { get; set; }

        [Column("situacao")]
        public Situacao Situacao { get; set; }

        public ICollection<Instituicao> Instituicoes { get; set; }

        public TipoInstituicao(int id, string descricao, Situacao situacao)
        {
             Id = id;
            Descricao = descricao;
            Situacao = situacao;
        }
    }
}
