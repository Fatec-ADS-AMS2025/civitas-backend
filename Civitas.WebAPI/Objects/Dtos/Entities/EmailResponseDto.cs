using Civitas.WebAPI.Objects.Enums;

namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    /// <summary>
    /// Representação de um e-mail enviado (ou com tentativa de envio) para o cliente da API.
    /// </summary>
    public class EmailResponseDto
    {
        public int Id { get; set; }

        public string EmailDestinatario { get; set; } = string.Empty;

        public string Assunto { get; set; } = string.Empty;

        public string Conteudo { get; set; } = string.Empty;

        public EmailStatusEnum Status { get; set; }

        public string? MensagemErro { get; set; }

        public DateTime? DataEnvio { get; set; }

        public DateTime DataCriacao { get; set; }
    }
}
