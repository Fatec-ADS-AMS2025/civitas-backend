using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Models
{
    [Table("token_recuperacao_senha")]
    public class PasswordResetToken
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Column("token_hash")]
        public string TokenHash { get; set; } = string.Empty;

        [Column("criado_em")]
        public DateTime CriadoEm { get; set; }

        [Column("expira_em")]
        public DateTime ExpiraEm { get; set; }

        [Column("utilizado_em")]
        public DateTime? UtilizadoEm { get; set; }

        public Usuario Usuario { get; set; } = null!;
    }
}
