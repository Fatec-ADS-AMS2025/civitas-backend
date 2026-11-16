using System.Globalization;
using System.Text.Json;
using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Services.Validation
{
    /// <summary>
    /// Centraliza as regras de validação dos registros de auditoria.
    /// </summary>
    /// <remarks>
    /// Usado pelo <c>AuditoriaService</c> (filtros de consulta) e pelo <c>AppDbContext</c>,
    /// que valida toda auditoria nova antes de gravá-la. Os erros são devolvidos juntos
    /// para que uma única falha descreva todos os problemas do registro.
    /// </remarks>
    public static class AuditoriaValidator
    {
        /// <summary>Formato exato da data (referência UTC).</summary>
        public const string FormatoData = "yyyy-MM-dd";

        /// <summary>Formato exato da hora, 24 horas (referência UTC).</summary>
        public const string FormatoHora = "HH:mm:ss";

        /// <summary>Tamanho máximo da operação, igual à coluna <c>operacao</c>.</summary>
        public const int TamanhoMaximoOperacao = 100;

        /// <summary>Tamanho máximo do nome da entidade, igual à coluna <c>nome_entidade</c>.</summary>
        public const int TamanhoMaximoNomeEntidade = 100;

        /// <summary>Tamanho máximo de cada snapshot, igual às colunas <c>valores_atingidos</c> e <c>novos_valores</c>.</summary>
        public const int TamanhoMaximoSnapshot = 10000;

        /// <summary>Valor gravado no lugar de dados sensíveis.</summary>
        public const string ValorMascarado = "***";

        /// <summary>Entidade lógica usada nos eventos de autenticação (LOGIN).</summary>
        public const string EntidadeAutenticacao = "Autenticacao";

        /// <summary>Entidades de negócio cujas alterações são auditadas.</summary>
        /// <remarks>
        /// <c>Auditoria</c>, <c>EmailEnviado</c> (log técnico de envio) e <c>PasswordResetToken</c>
        /// (dado sensível) não são auditadas.
        /// </remarks>
        public static readonly IReadOnlyList<string> EntidadesAuditaveis =
        [
            nameof(Usuario),
            nameof(Fornecedor),
            nameof(Secretaria),
            nameof(TipoInstituicao),
            nameof(Instituicao),
            nameof(UnidadeMedida),
            nameof(TipoDespesa),
            nameof(Orcamento),
            nameof(Despesa),
            nameof(TipoCodigo),
            nameof(UnidadeConsumidora)
        ];

        /// <summary>Propriedades que nunca podem aparecer sem máscara nos snapshots.</summary>
        public static readonly IReadOnlyList<string> PropriedadesSensiveis = ["Senha", "Token", "TokenHash"];

        /// <summary>
        /// Valida um registro de auditoria novo.
        /// </summary>
        /// <param name="auditoria">Registro a validar.</param>
        /// <returns>Lista de erros. Vazia quando o registro é válido.</returns>
        /// <remarks>A existência do usuário é conferida por quem tem acesso ao banco.</remarks>
        public static List<string> Validar(Auditoria auditoria)
        {
            var errors = new List<string>();

            if (auditoria.Id != 0)
            {
                errors.Add("O Id da auditoria é gerado pelo banco de dados e não pode ser informado.");
            }

            ValidarData(auditoria.Data, errors);
            ValidarHora(auditoria.Hora, errors);
            var operacaoValida = ValidarOperacao(auditoria.Operacao, errors);
            ValidarNomeEntidade(auditoria.NomeEntidade, operacaoValida, errors);

            var possuiAnterior = ValidarSnapshot(auditoria.ValoresAtingidos, nameof(Auditoria.ValoresAtingidos), errors);
            var possuiPosterior = ValidarSnapshot(auditoria.NovosValores, nameof(Auditoria.NovosValores), errors);

            if (operacaoValida is not null)
            {
                ValidarCompatibilidade(operacaoValida, possuiAnterior, possuiPosterior, errors);
            }

            if (auditoria.UsuarioId is <= 0)
            {
                errors.Add("O UsuarioId da auditoria deve ser maior que zero.");
            }

            return errors;
        }

        /// <summary>Valida e lança <see cref="AuditoriaValidationException"/> com todos os erros encontrados.</summary>
        public static void ValidarOuLancar(Auditoria auditoria)
        {
            var errors = Validar(auditoria);
            if (errors.Count > 0) throw new AuditoriaValidationException(errors);
        }

        /// <summary>Normaliza uma operação recebida em filtro (espaços e caixa). Retorna nulo se não for permitida.</summary>
        public static string? NormalizarOperacao(string? operacao)
        {
            var normalizada = operacao?.Trim().ToUpperInvariant();
            return AuditoriaOperacao.Todas.FirstOrDefault(item => item == normalizada);
        }

        /// <summary>Normaliza um nome de entidade recebido em filtro para a grafia oficial. Retorna nulo se não for permitido.</summary>
        public static string? NormalizarNomeEntidade(string? nomeEntidade)
        {
            var normalizado = nomeEntidade?.Trim();
            if (string.IsNullOrEmpty(normalizado)) return null;

            return EntidadesPermitidas().FirstOrDefault(item =>
                string.Equals(item, normalizado, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>Indica se a propriedade precisa ser mascarada nos snapshots.</summary>
        public static bool IsPropriedadeSensivel(string nomePropriedade) =>
            PropriedadesSensiveis.Any(item => string.Equals(item, nomePropriedade, StringComparison.OrdinalIgnoreCase));

        /// <summary>Mensagem padrão para operação fora da lista.</summary>
        public static string MensagemOperacaoInvalida(string? operacao) =>
            $"Operação '{operacao?.Trim()}' inválida. Operações permitidas: {string.Join(", ", AuditoriaOperacao.Todas)}.";

        /// <summary>Mensagem padrão para entidade fora da lista.</summary>
        public static string MensagemEntidadeInvalida(string? nomeEntidade) =>
            $"Entidade '{nomeEntidade?.Trim()}' inválida. Entidades permitidas: {string.Join(", ", EntidadesPermitidas())}.";

        private static IEnumerable<string> EntidadesPermitidas() => EntidadesAuditaveis.Append(EntidadeAutenticacao);

        private static void ValidarData(string? data, ICollection<string> errors)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                errors.Add("A data da auditoria é obrigatória.");
            }
            else if (!DateOnly.TryParseExact(data, FormatoData, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                errors.Add($"A data da auditoria deve ser uma data válida no formato {FormatoData}.");
            }
        }

        private static void ValidarHora(string? hora, ICollection<string> errors)
        {
            if (string.IsNullOrWhiteSpace(hora))
            {
                errors.Add("A hora da auditoria é obrigatória.");
            }
            else if (!TimeOnly.TryParseExact(hora, FormatoHora, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                errors.Add($"A hora da auditoria deve ser um horário válido no formato {FormatoHora}.");
            }
        }

        private static string? ValidarOperacao(string? operacao, ICollection<string> errors)
        {
            if (string.IsNullOrWhiteSpace(operacao))
            {
                errors.Add("A operação da auditoria é obrigatória.");
                return null;
            }

            if (operacao.Length > TamanhoMaximoOperacao)
            {
                errors.Add($"A operação deve ter no máximo {TamanhoMaximoOperacao} caracteres.");
                return null;
            }

            if (!AuditoriaOperacao.Todas.Contains(operacao))
            {
                errors.Add(MensagemOperacaoInvalida(operacao));
                return null;
            }

            return operacao;
        }

        private static void ValidarNomeEntidade(string? nomeEntidade, string? operacao, ICollection<string> errors)
        {
            if (string.IsNullOrWhiteSpace(nomeEntidade))
            {
                errors.Add("O nome da entidade é obrigatório.");
                return;
            }

            if (nomeEntidade.Length > TamanhoMaximoNomeEntidade)
            {
                errors.Add($"O nome da entidade deve ter no máximo {TamanhoMaximoNomeEntidade} caracteres.");
                return;
            }

            if (!EntidadesPermitidas().Contains(nomeEntidade))
            {
                errors.Add(MensagemEntidadeInvalida(nomeEntidade));
                return;
            }

            if (operacao == AuditoriaOperacao.LOGIN && nomeEntidade != EntidadeAutenticacao)
            {
                errors.Add($"A operação LOGIN deve usar a entidade {EntidadeAutenticacao}.");
            }
            else if (operacao is not null && operacao != AuditoriaOperacao.LOGIN && nomeEntidade == EntidadeAutenticacao)
            {
                errors.Add($"A entidade {EntidadeAutenticacao} só pode ser usada na operação LOGIN.");
            }
        }

        /// <returns>Verdadeiro quando o snapshot está preenchido.</returns>
        private static bool ValidarSnapshot(string? snapshot, string campo, ICollection<string> errors)
        {
            if (string.IsNullOrEmpty(snapshot)) return false;

            if (snapshot.Length > TamanhoMaximoSnapshot)
            {
                errors.Add($"{campo} deve ter no máximo {TamanhoMaximoSnapshot} caracteres.");
                return true;
            }

            try
            {
                using var documento = JsonDocument.Parse(snapshot);
                if (documento.RootElement.ValueKind != JsonValueKind.Object)
                {
                    errors.Add($"{campo} deve ser um objeto JSON.");
                    return true;
                }

                var sensiveis = PropriedadesSensiveisSemMascara(documento.RootElement).Distinct().ToList();
                if (sensiveis.Count > 0)
                {
                    errors.Add($"{campo} contém dados sensíveis sem máscara: {string.Join(", ", sensiveis)}.");
                }
            }
            catch (JsonException)
            {
                errors.Add($"{campo} contém JSON inválido.");
            }

            return true;
        }

        private static IEnumerable<string> PropriedadesSensiveisSemMascara(JsonElement elemento)
        {
            if (elemento.ValueKind == JsonValueKind.Object)
            {
                foreach (var propriedade in elemento.EnumerateObject())
                {
                    var valor = propriedade.Value;
                    var mascarado = valor.ValueKind == JsonValueKind.Null ||
                        (valor.ValueKind == JsonValueKind.String && valor.GetString() == ValorMascarado);

                    if (IsPropriedadeSensivel(propriedade.Name) && !mascarado)
                    {
                        yield return propriedade.Name;
                    }

                    foreach (var interna in PropriedadesSensiveisSemMascara(valor))
                    {
                        yield return interna;
                    }
                }
            }
            else if (elemento.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in elemento.EnumerateArray())
                {
                    foreach (var interna in PropriedadesSensiveisSemMascara(item))
                    {
                        yield return interna;
                    }
                }
            }
        }

        /// <summary>
        /// Aplica a tabela de obrigatoriedade dos snapshots:
        /// INSERT (vazio, obrigatório), UPDATE (obrigatório, obrigatório),
        /// DELETE (obrigatório, vazio) e LOGIN (vazio, vazio).
        /// </summary>
        private static void ValidarCompatibilidade(string operacao, bool possuiAnterior, bool possuiPosterior, ICollection<string> errors)
        {
            var (anteriorObrigatorio, posteriorObrigatorio) = operacao switch
            {
                AuditoriaOperacao.INSERT => (false, true),
                AuditoriaOperacao.UPDATE => (true, true),
                AuditoriaOperacao.DELETE => (true, false),
                _ => (false, false)
            };

            ValidarPresenca(operacao, nameof(Auditoria.ValoresAtingidos), anteriorObrigatorio, possuiAnterior, errors);
            ValidarPresenca(operacao, nameof(Auditoria.NovosValores), posteriorObrigatorio, possuiPosterior, errors);
        }

        private static void ValidarPresenca(string operacao, string campo, bool obrigatorio, bool preenchido, ICollection<string> errors)
        {
            if (obrigatorio && !preenchido)
            {
                errors.Add($"Na operação {operacao}, {campo} é obrigatório.");
            }
            else if (!obrigatorio && preenchido)
            {
                errors.Add($"Na operação {operacao}, {campo} deve ficar vazio.");
            }
        }
    }
}
