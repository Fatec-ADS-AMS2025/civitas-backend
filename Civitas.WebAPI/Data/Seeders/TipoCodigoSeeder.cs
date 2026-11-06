using Civitas.WebAPI.Data.Factories;

namespace Civitas.WebAPI.Data.Seeders
{
    /// <summary>Cria os tipos de código usados pelos tipos de despesa.</summary>
    public class TipoCodigoSeeder : Seeder
    {
        public const string Consumo = "CONSUMO";
        public const string Servico = "SERVICO";

        public TipoCodigoSeeder(AppDbContext context, IServiceProvider services) : base(context, services) { }

        public override async Task RunAsync(CancellationToken cancellationToken = default)
        {
            await TipoCodigoFactory.New()
                .State(t =>
                {
                    t.Nome = Consumo;
                    t.Descricao = "Despesas recorrentes vinculadas ao consumo medido.";
                })
                .CreateAsync(Context, cancellationToken);

            await TipoCodigoFactory.New()
                .State(t =>
                {
                    t.Nome = Servico;
                    t.Descricao = "Despesas operacionais e contratos de servicos.";
                })
                .CreateAsync(Context, cancellationToken);
        }
    }
}
