using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;
using Civitas.WebAPI.Services.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Civitas.WebAPI.Services.Entities
{
    /// <summary>
    /// Gera, a partir do ChangeTracker, os registros de auditoria das operações persistidas.
    /// </summary>
    /// <remarks>
    /// Estratégia de snapshot (sempre a mesma):
    /// - INSERT: todas as propriedades do registro criado em <c>NovosValores</c>; <c>ValoresAtingidos</c> vazio.
    /// - UPDATE: somente as propriedades alteradas, antes em <c>ValoresAtingidos</c> e depois em <c>NovosValores</c>.
    /// - DELETE: todas as propriedades do registro removido em <c>ValoresAtingidos</c>; <c>NovosValores</c> vazio.
    /// Senhas e tokens são gravados como "***". Data e hora vêm do mesmo instante UTC.
    /// </remarks>
    public sealed class AuditoriaCapturaService : IAuditoriaCapturaService
    {
        private static readonly JsonSerializerOptions SnapshotJsonOptions = new()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            Converters = { new JsonStringEnumConverter() }
        };

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditoriaNotificacaoService _notificacaoService;
        private readonly TimeProvider _timeProvider;

        public AuditoriaCapturaService(
            IHttpContextAccessor httpContextAccessor,
            IAuditoriaNotificacaoService notificacaoService,
            TimeProvider timeProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _notificacaoService = notificacaoService;
            _timeProvider = timeProvider;
        }

        /// <inheritdoc />
        public bool DeveAuditar => _httpContextAccessor.HttpContext is not null;

        /// <inheritdoc />
        public async Task<IReadOnlyList<AuditoriaPendente>> CapturarAsync(DbContext context, CancellationToken cancellationToken = default)
        {
            var pendentes = new List<AuditoriaPendente>();

            foreach (var entrada in context.ChangeTracker.Entries().ToList())
            {
                var nomeEntidade = entrada.Metadata.ClrType.Name;
                if (!AuditoriaValidator.EntidadesAuditaveis.Contains(nomeEntidade)) continue;

                switch (entrada.State)
                {
                    case EntityState.Added:
                        pendentes.Add(new AuditoriaPendente(entrada, AuditoriaOperacao.INSERT, nomeEntidade, null));
                        break;

                    case EntityState.Modified:
                    case EntityState.Deleted:
                        // O GenericRepository anexa entidades desconectadas; nesse caso os valores originais
                        // do ChangeTracker são iguais aos novos. O estado anterior real vem do banco.
                        var valoresBanco = await entrada.GetDatabaseValuesAsync(cancellationToken);
                        var anteriores = LerValores(entrada.Metadata, valoresBanco ?? entrada.OriginalValues);
                        var operacao = entrada.State == EntityState.Modified ? AuditoriaOperacao.UPDATE : AuditoriaOperacao.DELETE;
                        pendentes.Add(new AuditoriaPendente(entrada, operacao, nomeEntidade, anteriores));
                        break;
                }
            }

            return pendentes;
        }

        /// <inheritdoc />
        public IReadOnlyList<Auditoria> Gerar(IReadOnlyList<AuditoriaPendente> pendentes)
        {
            var (data, hora) = DataHoraAtual();
            var usuarioId = ObterUsuarioAutenticadoId();
            var auditorias = new List<Auditoria>();

            foreach (var pendente in pendentes)
            {
                string valoresAtingidos;
                string novosValores;

                switch (pendente.Operacao)
                {
                    case AuditoriaOperacao.INSERT:
                        valoresAtingidos = string.Empty;
                        novosValores = Serializar(LerValores(pendente.Entrada.Metadata, pendente.Entrada.CurrentValues));
                        break;

                    case AuditoriaOperacao.UPDATE:
                        var anteriores = pendente.ValoresAnteriores!;
                        var atuais = LerValores(pendente.Entrada.Metadata, pendente.Entrada.CurrentValues);
                        var alterados = atuais.Keys
                            .Where(chave => !ValoresIguais(anteriores.GetValueOrDefault(chave), atuais[chave]))
                            .ToList();

                        // Alteração sem diferença real não é uma operação a ser registrada.
                        if (alterados.Count == 0) continue;

                        valoresAtingidos = Serializar(alterados.ToDictionary(chave => chave, chave => anteriores.GetValueOrDefault(chave)));
                        novosValores = Serializar(alterados.ToDictionary(chave => chave, chave => atuais[chave]));
                        break;

                    default:
                        valoresAtingidos = Serializar(pendente.ValoresAnteriores!);
                        novosValores = string.Empty;
                        break;
                }

                auditorias.Add(new Auditoria(data, hora, pendente.Operacao, pendente.NomeEntidade,
                    valoresAtingidos, novosValores, usuarioId));
            }

            return auditorias;
        }

        /// <inheritdoc />
        public Auditoria CriarRegistro(string operacao, string nomeEntidade, string valoresAtingidos, string novosValores, int? usuarioId)
        {
            var (data, hora) = DataHoraAtual();
            return new Auditoria(data, hora, operacao, nomeEntidade, valoresAtingidos, novosValores, usuarioId);
        }

        /// <inheritdoc />
        public async Task PublicarAsync(IReadOnlyCollection<Auditoria> auditorias)
        {
            foreach (var auditoria in auditorias)
            {
                await _notificacaoService.PublicarCriadaAsync(new AuditoriaNotificacaoDTO
                {
                    Id = auditoria.Id,
                    Operacao = auditoria.Operacao,
                    NomeEntidade = auditoria.NomeEntidade,
                    Data = auditoria.Data,
                    Hora = auditoria.Hora,
                    NotificadoEmUtc = _timeProvider.GetUtcNow()
                });
            }
        }

        /// <summary>Data e hora UTC geradas do mesmo instante.</summary>
        private (string Data, string Hora) DataHoraAtual()
        {
            var agora = _timeProvider.GetUtcNow().UtcDateTime;
            return (
                agora.ToString(AuditoriaValidator.FormatoData, CultureInfo.InvariantCulture),
                agora.ToString(AuditoriaValidator.FormatoHora, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Lê o autor do token JWT validado pelo ASP.NET Core. Sem usuário autenticado retorna nulo;
        /// com token sem identificador numérico retorna 0, que a validação rejeita.
        /// </summary>
        private int? ObterUsuarioAutenticadoId()
        {
            var usuario = _httpContextAccessor.HttpContext?.User;
            if (usuario?.Identity?.IsAuthenticated != true) return null;

            var valor = usuario.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? usuario.FindFirstValue(JwtRegisteredClaimNames.Sub);

            return int.TryParse(valor, NumberStyles.None, CultureInfo.InvariantCulture, out var id) ? id : 0;
        }

        private static Dictionary<string, object?> LerValores(IEntityType tipo, PropertyValues valores)
        {
            var resultado = new Dictionary<string, object?>();

            foreach (var propriedade in tipo.GetProperties())
            {
                var valor = valores[propriedade];
                resultado[propriedade.Name] = AuditoriaValidator.IsPropriedadeSensivel(propriedade.Name) && valor is not null
                    ? new ValorSensivel(valor)
                    : valor;
            }

            return resultado;
        }

        private static string Serializar(IReadOnlyDictionary<string, object?> valores)
        {
            var mascarados = valores.ToDictionary(
                item => item.Key,
                item => item.Value is ValorSensivel ? AuditoriaValidator.ValorMascarado : item.Value);

            return JsonSerializer.Serialize(mascarados, SnapshotJsonOptions);
        }

        private static bool ValoresIguais(object? anterior, object? atual)
        {
            if (anterior is ValorSensivel sensivelAnterior) anterior = sensivelAnterior.Valor;
            if (atual is ValorSensivel sensivelAtual) atual = sensivelAtual.Valor;

            if (anterior is byte[] bytesAnteriores && atual is byte[] bytesAtuais)
            {
                return bytesAnteriores.AsSpan().SequenceEqual(bytesAtuais);
            }

            return Equals(anterior, atual);
        }

        /// <summary>Guarda o valor real para comparação, mas nunca é serializado.</summary>
        private sealed record ValorSensivel(object Valor);
    }
}
