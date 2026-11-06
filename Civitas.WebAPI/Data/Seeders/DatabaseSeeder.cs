using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Seeders
{
    /// <summary>
    /// Seeder raiz: executa todos os seeders na ordem das dependências entre as tabelas.
    /// </summary>
    /// <remarks>
    /// Não faz nada se o banco já tiver usuários. Para recriar tudo do zero use
    /// <c>dotnet run -- migrate:fresh --seed</c>.
    /// </remarks>
    public class DatabaseSeeder : Seeder
    {
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(AppDbContext context, IServiceProvider services, ILogger<DatabaseSeeder> logger)
            : base(context, services)
        {
            _logger = logger;
        }

        public override async Task RunAsync(CancellationToken cancellationToken = default)
        {
            var tabelaComDados = await FindTableWithDataAsync(cancellationToken);
            if (tabelaComDados is not null)
            {
                _logger.LogInformation(
                    "Seed ignorado: o banco ja possui dados (tabela '{Tabela}'). Use 'migrate:fresh --seed' para recriar.",
                    tabelaComDados);
                return;
            }

            _logger.LogInformation("Banco vazio: executando seed.");

            await CallAsync<UsuarioSeeder>(cancellationToken);
            await CallAsync<TipoCodigoSeeder>(cancellationToken);
            await CallAsync<UnidadeMedidaSeeder>(cancellationToken);
            await CallAsync<TipoInstituicaoSeeder>(cancellationToken);
            await CallAsync<TipoDespesaSeeder>(cancellationToken);
            await CallAsync<SecretariaSeeder>(cancellationToken);
            await CallAsync<FornecedorSeeder>(cancellationToken);
            await CallAsync<InstituicaoSeeder>(cancellationToken);
            await CallAsync<OrcamentoSeeder>(cancellationToken);
            await CallAsync<UnidadeConsumidoraSeeder>(cancellationToken);
            await CallAsync<DespesaSeeder>(cancellationToken);
            await CallAsync<AuditoriaSeeder>(cancellationToken);
            await CallAsync<EmailEnviadoSeeder>(cancellationToken);
            await CallAsync<PasswordResetTokenSeeder>(cancellationToken);

            _logger.LogInformation(
                "Seed concluido. Usuarios de teste: {Admin} / {Funcionario}. Senha: {Senha}",
                UsuarioSeeder.AdminEmail,
                UsuarioSeeder.FuncionarioEmail,
                Factories.UsuarioFactory.DefaultPassword);
        }

        /// <summary>Retorna o nome da primeira tabela do modelo que tem pelo menos uma linha, ou null se todas estiverem vazias.</summary>
        private async Task<string?> FindTableWithDataAsync(CancellationToken cancellationToken)
        {
            var tabelas = Context.Model.GetEntityTypes()
                .Select(e => e.GetTableName())
                .OfType<string>()
                .Distinct();

            foreach (var tabela in tabelas)
            {
                // Nome vem do modelo EF (não de entrada do usuário).
#pragma warning disable EF1002
                var temDados = await Context.Database
                    .SqlQueryRaw<bool>($"SELECT EXISTS (SELECT 1 FROM \"{tabela}\") AS \"Value\"")
                    .SingleAsync(cancellationToken);
#pragma warning restore EF1002

                if (temDados)
                {
                    return tabela;
                }
            }

            return null;
        }
    }
}
