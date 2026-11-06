using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Factories
{
    /// <summary>Factory de <see cref="EmailEnviado"/> (padrão: enviado com sucesso).</summary>
    public class EmailEnviadoFactory : Factory<EmailEnviado, EmailEnviadoFactory>
    {
        protected override EmailEnviado Definition()
        {
            var criadoEm = Faker.Date.Recent(30).ToUniversalTime();

            return new EmailEnviado
            {
                EmailDestinatario = Faker.Internet.Email().ToLowerInvariant(),
                Assunto = Faker.Lorem.Sentence(4),
                Conteudo = $"<p>{Faker.Lorem.Paragraph()}</p>",
                Status = EmailStatusEnum.ENVIADO,
                DataCriacao = criadoEm,
                DataEnvio = criadoEm.AddSeconds(Faker.Random.Int(1, 30)),
                MensagemErro = null
            };
        }

        public EmailEnviadoFactory Pendente() => State(e =>
        {
            e.Status = EmailStatusEnum.PENDENTE;
            e.DataEnvio = null;
            e.MensagemErro = null;
        });

        public EmailEnviadoFactory Enviado() => State(e =>
        {
            e.Status = EmailStatusEnum.ENVIADO;
            e.DataEnvio ??= e.DataCriacao.AddSeconds(5);
            e.MensagemErro = null;
        });

        public EmailEnviadoFactory ComFalha(string mensagemErro = "Falha de conexao com o servidor SMTP.") => State(e =>
        {
            e.Status = EmailStatusEnum.FALHA;
            e.DataEnvio = null;
            e.MensagemErro = mensagemErro;
        });

        public EmailEnviadoFactory Para(string emailDestinatario) => State(e => e.EmailDestinatario = emailDestinatario);
    }
}
