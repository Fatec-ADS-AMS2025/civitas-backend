using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;
using Civitas.WebAPI.Services.Validation;

namespace Civitas.WebAPI.Services.Entities
{
    public class EmailService : IEmailService
    {
        public const int AssuntoMaximo = 200;
        public const int ConteudoMaximo = 10000;
        public const int EmailMaximo = 255;

        private static readonly EmailAddressAttribute EmailValidator = new();

        private readonly IEmailRepository _emailRepository;
        private readonly IEmailSender _emailSender;
        private readonly IMapper _mapper;

        public EmailService(IEmailRepository emailRepository, IEmailSender emailSender, IMapper mapper)
        {
            _emailRepository = emailRepository;
            _emailSender = emailSender;
            _mapper = mapper;
        }

        public async Task<EmailResponseDto> EnviarAsync(EmailEnvioDto dto)
        {
            if (dto is null)
            {
                throw new EmailValidationException(["O corpo da requisição é obrigatório."]);
            }

            var destinatarioNormalizado = NormalizarEmail(dto.EmailDestinatario);
            var assunto = dto.Assunto?.Trim() ?? string.Empty;
            var conteudo = dto.Conteudo?.Trim() ?? string.Empty;

            var errors = new List<string>();
            ValidarDestinatario(dto.EmailDestinatario, destinatarioNormalizado, errors);
            ValidarAssunto(assunto, errors);
            ValidarConteudo(conteudo, errors);

            if (errors.Count > 0)
            {
                throw new EmailValidationException(errors);
            }

            var agora = DateTime.UtcNow;
            var entity = new EmailEnviado
            {
                EmailDestinatario = destinatarioNormalizado,
                Assunto = assunto,
                Conteudo = conteudo,
                Status = EmailStatusEnum.PENDENTE,
                DataCriacao = agora
            };

            await _emailRepository.Add(entity);

            try
            {
                await _emailSender.SendAsync(destinatarioNormalizado, assunto, conteudo);
                entity.Status = EmailStatusEnum.ENVIADO;
                entity.DataEnvio = DateTime.UtcNow;
                entity.MensagemErro = null;
                await _emailRepository.Update(entity);
                return Mapear(entity);
            }
            catch (Exception ex) when (ex is not EmailValidationException)
            {
                entity.Status = EmailStatusEnum.FALHA;
                entity.MensagemErro = "Falha de comunicação com o provedor de e-mail.";
                await _emailRepository.Update(entity);
                throw new EmailSendException(
                    "Não foi possível enviar o e-mail.",
                    Mapear(entity),
                    ex);
            }
        }

        public async Task<IReadOnlyList<EmailResponseDto>> ListarAsync(EmailFiltroDto? filtro = null)
        {
            if (filtro is not null && !string.IsNullOrWhiteSpace(filtro.EmailDestinatario))
            {
                return await ListarPorDestinatarioAsync(filtro.EmailDestinatario);
            }

            if (filtro is not null && !string.IsNullOrWhiteSpace(filtro.Status))
            {
                return await ListarPorStatusAsync(filtro.Status);
            }

            var emails = await _emailRepository.Get();
            return emails.Select(Mapear).ToList();
        }

        public async Task<EmailResponseDto> ObterPorIdAsync(int id)
        {
            var email = await _emailRepository.GetById(id);
            if (email is null)
            {
                throw new KeyNotFoundException("O e-mail enviado informado não foi encontrado.");
            }

            return Mapear(email);
        }

        public async Task<IReadOnlyList<EmailResponseDto>> ListarPorDestinatarioAsync(string email)
        {
            var destinatarioNormalizado = NormalizarEmail(email);
            var errors = new List<string>();
            ValidarDestinatario(email, destinatarioNormalizado, errors);

            if (errors.Count > 0)
            {
                throw new EmailValidationException(errors);
            }

            var emails = await _emailRepository.GetByDestinatarioAsync(destinatarioNormalizado);
            return emails.Select(Mapear).ToList();
        }

        public async Task<IReadOnlyList<EmailResponseDto>> ListarPorStatusAsync(string status)
        {
            if (!TryParseStatus(status, out var statusEnum))
            {
                throw new EmailValidationException([
                    "O status informado é inválido. Valores permitidos: 1 (PENDENTE), 2 (ENVIADO) ou 3 (FALHA)."
                ]);
            }

            var emails = await _emailRepository.GetByStatusAsync(statusEnum);
            return emails.Select(Mapear).ToList();
        }

        private EmailResponseDto Mapear(EmailEnviado entity)
        {
            return _mapper.Map<EmailResponseDto>(entity);
        }

        private static void ValidarDestinatario(string? original, string normalizado, ICollection<string> errors)
        {
            if (string.IsNullOrWhiteSpace(original))
            {
                errors.Add("O campo EmailDestinatario é obrigatório.");
                return;
            }

            var trimmed = original.Trim();
            if (trimmed.Any(char.IsWhiteSpace))
            {
                errors.Add("O e-mail do destinatário não pode conter espaços em branco.");
            }

            if (normalizado.Length > EmailMaximo)
            {
                errors.Add($"O e-mail do destinatário deve ter no máximo {EmailMaximo} caracteres.");
            }

            if (!EhEmailValido(normalizado))
            {
                errors.Add("O e-mail do destinatário informado é inválido.");
            }
        }

        private static void ValidarAssunto(string assunto, ICollection<string> errors)
        {
            if (string.IsNullOrWhiteSpace(assunto))
            {
                errors.Add("O campo Assunto é obrigatório.");
                return;
            }

            if (assunto.Length > AssuntoMaximo)
            {
                errors.Add($"O assunto deve ter no máximo {AssuntoMaximo} caracteres.");
            }
        }

        private static void ValidarConteudo(string conteudo, ICollection<string> errors)
        {
            if (string.IsNullOrWhiteSpace(conteudo))
            {
                errors.Add("O campo Conteudo é obrigatório.");
                return;
            }

            if (conteudo.Length > ConteudoMaximo)
            {
                errors.Add($"O conteúdo deve ter no máximo {ConteudoMaximo} caracteres.");
            }
        }

        private static string NormalizarEmail(string? email)
        {
            return email?.Trim().ToLowerInvariant() ?? string.Empty;
        }

        private static bool EhEmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || email.Any(char.IsWhiteSpace))
            {
                return false;
            }

            if (!EmailValidator.IsValid(email))
            {
                return false;
            }

            try
            {
                var mailAddress = new MailAddress(email);
                return string.Equals(mailAddress.Address, email, StringComparison.OrdinalIgnoreCase);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private static bool TryParseStatus(string? status, out EmailStatusEnum statusEnum)
        {
            statusEnum = default;

            if (string.IsNullOrWhiteSpace(status))
            {
                return false;
            }

            var valor = status.Trim();
            if (int.TryParse(valor, out var numero) && Enum.IsDefined(typeof(EmailStatusEnum), numero))
            {
                statusEnum = (EmailStatusEnum)numero;
                return true;
            }

            return Enum.TryParse(valor, ignoreCase: true, out statusEnum)
                && Enum.IsDefined(statusEnum);
        }
    }
}
