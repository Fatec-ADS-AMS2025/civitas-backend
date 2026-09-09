namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    /// <summary>
    /// Dados necessários para disparar um e-mail pela API.
    /// </summary>
    public class EmailEnvioDto
    {
        /// <summary>
        /// Destinatário. Será normalizado (trim + minúsculas) antes do envio.
        /// </summary>
        /// <example>usuario@prefeitura.gov.br</example>
        public string EmailDestinatario { get; set; } = string.Empty;

        /// <summary>
        /// Assunto. Máximo de 200 caracteres após o trim.
        /// </summary>
        public string Assunto { get; set; } = string.Empty;

        /// <summary>
        /// Corpo da mensagem em texto simples. Máximo de 10000 caracteres após o trim.
        /// </summary>
        public string Conteudo { get; set; } = string.Empty;
    }
}
