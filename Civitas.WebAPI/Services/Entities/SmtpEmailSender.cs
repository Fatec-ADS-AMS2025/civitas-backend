using Civitas.WebAPI.Services.Interfaces;
using Civitas.WebAPI.Services.Security;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Civitas.WebAPI.Services.Entities
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly EmailOptions _options;

        public SmtpEmailSender(IOptions<EmailOptions> options)
        {
            _options = options.Value;
        }

        public async Task SendAsync(
            string destinatario,
            string assunto,
            string conteudo,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_options.Host) || string.IsNullOrWhiteSpace(_options.FromAddress))
            {
                throw new InvalidOperationException("As configurações de e-mail da aplicação estão incompletas.");
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_options.FromName, _options.FromAddress));
            message.To.Add(MailboxAddress.Parse(destinatario));
            message.Subject = assunto;
            message.Body = new TextPart("plain") { Text = conteudo };

            using var client = new SmtpClient();
            client.Timeout = Math.Max(_options.TimeoutSeconds, 1) * 1000;

            var socketOptions = ResolveSocketOptions();
            await client.ConnectAsync(_options.Host, _options.Port, socketOptions, cancellationToken);

            if (!string.IsNullOrWhiteSpace(_options.Username))
            {
                await client.AuthenticateAsync(_options.Username, _options.Password, cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }

        private SecureSocketOptions ResolveSocketOptions()
        {
            if (!_options.EnableSsl)
            {
                return SecureSocketOptions.None;
            }

            return _options.Port == 465
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTls;
        }
    }
}
