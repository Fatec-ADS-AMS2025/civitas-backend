namespace Civitas.WebAPI.Services.Interfaces
{
    /// <summary>
    /// Abstração do provedor SMTP usado pelo módulo de e-mail.
    /// </summary>
    public interface IEmailSender
    {
        Task SendAsync(
            string destinatario,
            string assunto,
            string conteudo,
            CancellationToken cancellationToken = default);
    }
}
