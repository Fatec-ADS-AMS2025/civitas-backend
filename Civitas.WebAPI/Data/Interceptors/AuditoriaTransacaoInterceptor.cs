using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Civitas.WebAPI.Data.Interceptors
{
    /// <summary>
    /// Publica as notificações de auditoria somente quando a transação que as gravou é confirmada.
    /// </summary>
    /// <remarks>
    /// Vale tanto para a transação aberta pelo próprio <see cref="AppDbContext"/> quanto para as
    /// transações abertas pelos serviços (DespesaService, OrcamentoService, AuthService).
    /// Em rollback, falha ou início de outra transação, as notificações pendentes são descartadas.
    /// Não guarda estado: a fila fica no próprio DbContext, por isso uma única instância é compartilhada.
    /// </remarks>
    public sealed class AuditoriaTransacaoInterceptor : DbTransactionInterceptor
    {
        /// <summary>Instância única registrada no <see cref="AppDbContext"/>.</summary>
        public static readonly AuditoriaTransacaoInterceptor Instancia = new();

        private AuditoriaTransacaoInterceptor()
        {
        }

        public override async Task TransactionCommittedAsync(DbTransaction transaction, TransactionEndEventData eventData,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context is AppDbContext context)
            {
                await context.PublicarNotificacoesPendentesAsync();
            }
        }

        public override void TransactionCommitted(DbTransaction transaction, TransactionEndEventData eventData)
        {
            if (eventData.Context is AppDbContext context)
            {
                context.PublicarNotificacoesPendentesAsync().GetAwaiter().GetResult();
            }
        }

        public override Task TransactionRolledBackAsync(DbTransaction transaction, TransactionEndEventData eventData,
            CancellationToken cancellationToken = default)
        {
            Descartar(eventData);
            return Task.CompletedTask;
        }

        public override void TransactionRolledBack(DbTransaction transaction, TransactionEndEventData eventData)
            => Descartar(eventData);

        public override Task TransactionFailedAsync(DbTransaction transaction, TransactionErrorEventData eventData,
            CancellationToken cancellationToken = default)
        {
            Descartar(eventData);
            return Task.CompletedTask;
        }

        public override void TransactionFailed(DbTransaction transaction, TransactionErrorEventData eventData)
            => Descartar(eventData);

        public override ValueTask<InterceptionResult<DbTransaction>> TransactionStartingAsync(DbConnection connection,
            TransactionStartingEventData eventData, InterceptionResult<DbTransaction> result,
            CancellationToken cancellationToken = default)
        {
            Descartar(eventData);
            return ValueTask.FromResult(result);
        }

        public override InterceptionResult<DbTransaction> TransactionStarting(DbConnection connection,
            TransactionStartingEventData eventData, InterceptionResult<DbTransaction> result)
        {
            Descartar(eventData);
            return result;
        }

        private static void Descartar(DbContextEventData eventData)
        {
            (eventData.Context as AppDbContext)?.DescartarNotificacoesPendentes();
        }
    }
}
