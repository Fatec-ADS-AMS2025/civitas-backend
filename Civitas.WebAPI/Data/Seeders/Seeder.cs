namespace Civitas.WebAPI.Data.Seeders
{
    /// <summary>
    /// Base dos seeders, no estilo do Laravel: cada seeder popula uma tabela em <see cref="RunAsync"/>
    /// e pode chamar outros seeders com <see cref="CallAsync{TSeeder}"/>.
    /// </summary>
    /// <remarks>
    /// Seeders são instanciados via <see cref="ActivatorUtilities"/>, então podem receber
    /// qualquer serviço registrado no construtor sem precisar de registro próprio no DI.
    /// </remarks>
    public abstract class Seeder
    {
        protected Seeder(AppDbContext context, IServiceProvider services)
        {
            Context = context;
            Services = services;
        }

        protected AppDbContext Context { get; }

        protected IServiceProvider Services { get; }

        /// <summary>Popula os dados do seeder.</summary>
        public abstract Task RunAsync(CancellationToken cancellationToken = default);

        /// <summary>Executa outro seeder.</summary>
        protected Task CallAsync<TSeeder>(CancellationToken cancellationToken = default) where TSeeder : Seeder
            => RunSeederAsync(Services, typeof(TSeeder), cancellationToken);

        /// <summary>Instancia e executa o seeder do tipo informado.</summary>
        public static async Task RunSeederAsync(IServiceProvider services, Type seederType, CancellationToken cancellationToken = default)
        {
            if (!typeof(Seeder).IsAssignableFrom(seederType) || seederType.IsAbstract)
            {
                throw new ArgumentException($"{seederType.Name} nao e um seeder valido.", nameof(seederType));
            }

            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(seederType);
            var seeder = (Seeder)ActivatorUtilities.CreateInstance(services, seederType);

            logger.LogInformation("Seeding: {Seeder}", seederType.Name);
            await seeder.RunAsync(cancellationToken);
        }
    }
}
