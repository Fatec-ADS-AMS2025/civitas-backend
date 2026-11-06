using Civitas.WebAPI.Data.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Civitas.WebAPI.Data
{
    /// <summary>
    /// Comandos de banco no estilo do Artisan do Laravel, executados com
    /// <c>dotnet run -- &lt;comando&gt;</c> (ou <c>docker compose run --rm --no-deps backend &lt;comando&gt;</c> no container).
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><c>migrate</c> — aplica as migrations pendentes.</item>
    /// <item><c>migrate:status</c> — lista migrations aplicadas e pendentes.</item>
    /// <item><c>migrate:rollback [--step=N]</c> — desfaz as últimas N migrations (padrão 1).</item>
    /// <item><c>migrate:fresh [--seed]</c> — apaga todas as tabelas, migra do zero e opcionalmente popula.</item>
    /// <item><c>db:seed [--class=NomeSeeder]</c> — executa o DatabaseSeeder ou um seeder específico.</item>
    /// </list>
    /// </remarks>
    public static class DatabaseCommands
    {
        private static readonly string[] Commands = ["migrate", "migrate:status", "migrate:rollback", "migrate:fresh", "db:seed"];

        /// <summary>Indica se os argumentos de linha de comando pedem um comando de banco.</summary>
        public static bool IsCommand(string[] args) => args.Length > 0 && Commands.Contains(args[0]);

        /// <summary>Executa o comando e retorna o exit code do processo.</summary>
        public static async Task<int> RunAsync(IServiceProvider rootServices, string[] args)
        {
            using var scope = rootServices.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<AppDbContext>();
            var options = args.Skip(1).ToArray();

            try
            {
                switch (args[0])
                {
                    case "migrate":
                        await MigrateAsync(context);
                        break;

                    case "migrate:status":
                        await StatusAsync(context);
                        break;

                    case "migrate:rollback":
                        await RollbackAsync(context, int.Parse(GetOption(options, "step") ?? "1"));
                        break;

                    case "migrate:fresh":
                        await FreshAsync(context);
                        if (options.Contains("--seed"))
                        {
                            await SeedAsync(services, GetOption(options, "class"));
                        }
                        break;

                    case "db:seed":
                        await SeedAsync(services, GetOption(options, "class"));
                        break;
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erro ao executar '{args[0]}': {ex.Message}");
                return 1;
            }
        }

        /// <summary>
        /// Aplica as migrations pendentes. Falha com instrução clara se o banco foi criado
        /// pelo antigo <c>EnsureCreated</c> (tabelas existentes sem histórico de migrations).
        /// </summary>
        public static async Task MigrateAsync(AppDbContext context, CancellationToken cancellationToken = default)
        {
            var historyExists = await HistoryTableExistsAsync(context, cancellationToken);
            if (!historyExists && await TableExistsAsync(context, "usuario", cancellationToken))
            {
                throw new InvalidOperationException(
                    "O banco ja tem tabelas mas nenhuma migration registrada (foi criado com EnsureCreated ou scripts SQL). " +
                    "Recrie com 'docker compose run --rm --no-deps backend migrate:fresh --seed' (docker) ou 'dotnet run -- migrate:fresh --seed' (local).");
            }

            var pending = historyExists
                ? (await context.Database.GetPendingMigrationsAsync(cancellationToken)).ToList()
                : context.Database.GetMigrations().ToList();
            await context.Database.MigrateAsync(cancellationToken);

            Console.WriteLine(pending.Count == 0 ? "Nada para migrar." : $"Migrado: {string.Join(", ", pending)}");
        }

        private static async Task StatusAsync(AppDbContext context)
        {
            var applied = await HistoryTableExistsAsync(context)
                ? (await context.Database.GetAppliedMigrationsAsync()).ToHashSet()
                : [];
            foreach (var migration in context.Database.GetMigrations())
            {
                Console.WriteLine($"{(applied.Contains(migration) ? "[x]" : "[ ]")} {migration}");
            }
        }

        private static async Task RollbackAsync(AppDbContext context, int steps)
        {
            var applied = (await context.Database.GetAppliedMigrationsAsync()).ToList();
            if (applied.Count == 0)
            {
                Console.WriteLine("Nada para desfazer.");
                return;
            }

            var targetIndex = applied.Count - 1 - steps;
            var target = targetIndex >= 0 ? applied[targetIndex] : Migration.InitialDatabase;

            await context.GetService<IMigrator>().MigrateAsync(target);
            Console.WriteLine($"Desfeito: {string.Join(", ", applied.Skip(Math.Max(targetIndex + 1, 0)).Reverse())}");
        }

        private static async Task FreshAsync(AppDbContext context)
        {
            // Equivalente ao migrate:fresh do Laravel: derruba todas as tabelas do schema e migra do zero.
            await context.Database.ExecuteSqlRawAsync("DROP SCHEMA IF EXISTS public CASCADE; CREATE SCHEMA public;");
            Console.WriteLine("Todas as tabelas foram removidas.");
            await context.Database.MigrateAsync();
            Console.WriteLine("Migrations aplicadas do zero.");
        }

        private static async Task SeedAsync(IServiceProvider services, string? seederName)
        {
            var seederType = typeof(DatabaseSeeder);
            if (seederName is not null)
            {
                seederType = typeof(Seeder).Assembly.GetTypes()
                    .SingleOrDefault(t => typeof(Seeder).IsAssignableFrom(t) && !t.IsAbstract && t.Name == seederName)
                    ?? throw new ArgumentException($"Seeder '{seederName}' nao encontrado.");
            }

            await Seeder.RunSeederAsync(services, seederType);
            Console.WriteLine($"Seed concluido: {seederType.Name}");
        }

        // IHistoryRepository.ExistsAsync do Npgsql 9 retorna true mesmo sem a tabela; consulta direto.
        private static Task<bool> HistoryTableExistsAsync(AppDbContext context, CancellationToken cancellationToken = default)
            => TableExistsAsync(context, HistoryRepository.DefaultTableName, cancellationToken);

        private static async Task<bool> TableExistsAsync(AppDbContext context, string table, CancellationToken cancellationToken)
        {
            var count = await context.Database
                .SqlQuery<int>($"SELECT COUNT(*)::int AS \"Value\" FROM information_schema.tables WHERE table_schema = 'public' AND table_name = {table}")
                .SingleAsync(cancellationToken);
            return count > 0;
        }

        private static string? GetOption(string[] options, string name)
        {
            var prefix = $"--{name}=";
            return options.FirstOrDefault(o => o.StartsWith(prefix))?[prefix.Length..];
        }
    }
}
