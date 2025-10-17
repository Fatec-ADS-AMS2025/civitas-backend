using Civitas.WebAPI.Objects.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Models
{
    [Table("auditoria")]
    public class Auditoria
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("data")]
        public string Data { get; set; } = string.Empty;

        [Column("hora")]
        public string Hora { get; set; } = string.Empty;

        [Column("operacao")]
        public string Operacao { get; set; } = string.Empty;

        [Column("nome_entidade")]
        public string NomeEntidade { get; set; } = string.Empty;

        [Column("valores_atingidos")]
        public string ValoresAtingidos { get; set; } = string.Empty;

        [Column("novos_valores")]
        public string NovosValores { get; set; } = string.Empty;

        [Column("situacao")]
        public Situacao Situacao { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }

        public Auditoria()
        {
        }

        public Auditoria(int id, string data, string hora, string operacao, string nomeEntidade, 
            string valoresAtingidos, string novosValores, Situacao situacao, int usuarioId)
        {
            Id = id;
            Data = data;
            Hora = hora;
            Operacao = operacao;
            NomeEntidade = nomeEntidade;
            ValoresAtingidos = valoresAtingidos;
            NovosValores = novosValores;
            Situacao = situacao;
            UsuarioId = usuarioId;
        }
    }
}
