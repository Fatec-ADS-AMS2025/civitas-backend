using System.ComponentModel.DataAnnotations.Schema;
using Civitas.WebAPI.Objects.Enums;

namespace Civitas.WebAPI.Objects.Models
{
    /// <summary>
    /// Registro de tentativa de envio de e-mail persistido para auditoria.
    /// </summary>
    [Table("emailenviado")]
    public class EmailEnviado
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("emaildestinatario")]
        public string EmailDestinatario { get; set; } = string.Empty;

        [Column("assunto")]
        public string Assunto { get; set; } = string.Empty;

        [Column("conteudo")]
        public string Conteudo { get; set; } = string.Empty;

        [Column("status")]
        public EmailStatusEnum Status { get; set; }

        [Column("mensagemerro")]
        public string? MensagemErro { get; set; }

        [Column("dataenvio")]
        public DateTime? DataEnvio { get; set; }

        [Column("datacriacao")]
        public DateTime DataCriacao { get; set; }

        public EmailEnviado()
        {
        }

        public EmailEnviado(
            int id,
            string emailDestinatario,
            string assunto,
            string conteudo,
            EmailStatusEnum status,
            DateTime dataCriacao,
            string? mensagemErro = null,
            DateTime? dataEnvio = null)
        {
            Id = id;
            EmailDestinatario = emailDestinatario;
            Assunto = assunto;
            Conteudo = conteudo;
            Status = status;
            DataCriacao = dataCriacao;
            MensagemErro = mensagemErro;
            DataEnvio = dataEnvio;
        }
    }
}
