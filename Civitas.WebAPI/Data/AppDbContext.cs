using Civitas.WebAPI.Data.Builders;
using Civitas.WebAPI.Data.Interceptors;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;
using Civitas.WebAPI.Services.Validation;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IAuditoriaCapturaService? _auditoriaCaptura;
        private readonly List<Auditoria> _notificacoesPendentes = [];

        /// <param name="options">Configuração do Entity Framework.</param>
        /// <param name="auditoriaCaptura">
        /// Geração automática de auditoria. Opcional para permitir criar o contexto fora da injeção
        /// de dependências (testes de repositório); sem ele, só a imutabilidade e a validação continuam ativas.
        /// </param>
        public AppDbContext(DbContextOptions<AppDbContext> options, IAuditoriaCapturaService? auditoriaCaptura = null)
            : base(options)
        {
            _auditoriaCaptura = auditoriaCaptura;
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }
        public DbSet<Secretaria> Secretarias { get; set; }
        public DbSet<Auditoria> Auditorias { get; set; }
        public DbSet<TipoInstituicao> TipoInstituicoes { get; set; }
        public DbSet<Instituicao> Instituicoes { get; set; }
        public DbSet<UnidadeMedida> UnidadesMedida { get; set; }
        public DbSet<TipoDespesa> TiposDespesa { get; set; }
        public DbSet<Orcamento> Orcamentos { get; set; }
        public DbSet<Despesa> Despesas { get; set; }
        public DbSet<TipoCodigo> TipoCodigos { get; set; }
        public DbSet<UnidadeConsumidora> UnidadesConsumidoras { get; set; }
        public DbSet<EmailEnviado> EmailsEnviados { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(AuditoriaTransacaoInterceptor.Instancia);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            UsuarioBuilder.Build(modelBuilder);
            FornecedorBuilder.Build(modelBuilder);
            SecretariaBuilder.Build(modelBuilder);
            AuditoriaBuilder.Build(modelBuilder);
            TipoInstituicaoBuilder.Build(modelBuilder);
            InstituicaoBuilder.Build(modelBuilder);
            UnidadeMedidaBuilder.Build(modelBuilder);
            TipoDespesaBuilder.Build(modelBuilder);
            OrcamentoBuilder.Build(modelBuilder);
            UnidadeConsumidoraBuilder.Build(modelBuilder);
            DespesaBuilder.Build(modelBuilder);
            EmailEnviadoBuilder.Build(modelBuilder);
            PasswordResetTokenBuilder.Build(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                         .Where(entityType => typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType)))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(ISoftDeletable.Excluido))
                    .HasColumnName("excluido")
                    .HasDefaultValue(false);

                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(ISoftDeletable.DataExclusao))
                    .HasColumnName("dataexclusao");
            }
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return SaveChangesAsync(acceptAllChangesOnSuccess).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Grava as alterações e, na mesma transação, as auditorias correspondentes.
        /// </summary>
        /// <remarks>
        /// 1. Bloqueia alteração ou exclusão de auditorias já gravadas.
        /// 2. Captura as entidades auditáveis alteradas e o estado anterior real (lido do banco).
        /// 3. Grava a operação principal, gera as auditorias (com os Ids já definidos), valida e grava.
        /// 4. Confirma a transação própria ou deixa a confirmação para a transação externa do serviço.
        /// As notificações SignalR só saem no commit (ver <see cref="AuditoriaTransacaoInterceptor"/>).
        /// Qualquer falha na auditoria desfaz a operação principal e é tratada como erro interno.
        /// </remarks>
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            GarantirImutabilidadeDasAuditorias();

            var deveAuditar = _auditoriaCaptura?.DeveAuditar == true;
            var pendentes = deveAuditar
                ? await _auditoriaCaptura!.CapturarAsync(this, cancellationToken)
                : [];
            var auditoriasNovas = ChangeTracker.Entries<Auditoria>()
                .Where(entrada => entrada.State == EntityState.Added)
                .Select(entrada => entrada.Entity)
                .ToList();

            if (pendentes.Count == 0 && auditoriasNovas.Count == 0)
            {
                return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            }

            await ValidarAuditoriasAsync(auditoriasNovas, cancellationToken);

            var transacaoPropria = Database.CurrentTransaction is null
                ? await Database.BeginTransactionAsync(cancellationToken)
                : null;

            try
            {
                // As entidades precisam ser aceitas para que a segunda gravação não as repita.
                var total = await base.SaveChangesAsync(acceptAllChangesOnSuccess: true, cancellationToken);

                if (pendentes.Count > 0)
                {
                    var geradas = _auditoriaCaptura!.Gerar(pendentes);
                    if (geradas.Count > 0)
                    {
                        await ValidarAuditoriasAsync(geradas, cancellationToken);
                        Auditorias.AddRange(geradas);
                        total += await base.SaveChangesAsync(acceptAllChangesOnSuccess: true, cancellationToken);
                        auditoriasNovas.AddRange(geradas);
                    }
                }

                if (deveAuditar)
                {
                    _notificacoesPendentes.AddRange(auditoriasNovas.Where(auditoria => !_notificacoesPendentes.Contains(auditoria)));
                }

                if (transacaoPropria is not null)
                {
                    await transacaoPropria.CommitAsync(cancellationToken);
                }

                return total;
            }
            catch
            {
                if (transacaoPropria is not null)
                {
                    DescartarNotificacoesPendentes();
                }

                throw;
            }
            finally
            {
                if (transacaoPropria is not null)
                {
                    await transacaoPropria.DisposeAsync();
                }
            }
        }

        /// <summary>Publica, uma única vez, as auditorias confirmadas pela transação.</summary>
        internal async Task PublicarNotificacoesPendentesAsync()
        {
            if (_auditoriaCaptura is null || _notificacoesPendentes.Count == 0) return;

            var lote = _notificacoesPendentes.ToList();
            _notificacoesPendentes.Clear();
            await _auditoriaCaptura.PublicarAsync(lote);
        }

        /// <summary>Remove notificações de auditorias que não foram confirmadas.</summary>
        internal void DescartarNotificacoesPendentes() => _notificacoesPendentes.Clear();

        private void GarantirImutabilidadeDasAuditorias()
        {
            if (ChangeTracker.Entries<Auditoria>().Any(entrada => entrada.State is EntityState.Modified or EntityState.Deleted))
            {
                throw new InvalidOperationException("Registros de auditoria são imutáveis e não podem ser alterados ou excluídos.");
            }
        }

        /// <summary>
        /// Valida auditorias novas. Uma auditoria inválida é falha interna do servidor,
        /// não erro de quem fez a requisição, por isso o erro sai como <see cref="InvalidOperationException"/>.
        /// </summary>
        private async Task ValidarAuditoriasAsync(IEnumerable<Auditoria> auditorias, CancellationToken cancellationToken)
        {
            var errors = new List<string>();

            foreach (var auditoria in auditorias)
            {
                errors.AddRange(AuditoriaValidator.Validar(auditoria));

                if (auditoria.UsuarioId is int usuarioId && usuarioId > 0 && auditoria.Usuario is null &&
                    !await Usuarios.AsNoTracking().AnyAsync(usuario => usuario.Id == usuarioId, cancellationToken))
                {
                    errors.Add($"O usuário {usuarioId} informado na auditoria não existe.");
                }
            }

            if (errors.Count > 0)
            {
                throw new InvalidOperationException(
                    "Falha interna ao registrar a auditoria da operação.",
                    new AuditoriaValidationException(errors.Distinct()));
            }
        }
    }
}
