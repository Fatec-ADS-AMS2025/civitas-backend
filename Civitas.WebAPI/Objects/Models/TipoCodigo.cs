using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Models
{
    [Table("tipocodigo")]
    public class TipoCodigo
    {
        [Key]
        [Required]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nome")]
        public string Nome { get; set; }

        [Required]
        [Column("descricao")]
        public string Descricao { get; set; }

        public ICollection<TipoDespesa> TipoDespesas { get; set; }


        public TipoCodigo()
        {
            
        }

        public TipoCodigo(int id, string nome, string descricao)
        {
            Id = id;
            Nome = nome;
            Descricao = descricao;
        }
    }
}
