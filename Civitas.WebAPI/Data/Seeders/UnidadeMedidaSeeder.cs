using Civitas.WebAPI.Data.Factories;

namespace Civitas.WebAPI.Data.Seeders
{
    /// <summary>Cria as unidades de medida básicas.</summary>
    public class UnidadeMedidaSeeder : Seeder
    {
        public UnidadeMedidaSeeder(AppDbContext context, IServiceProvider services) : base(context, services) { }

        public override async Task RunAsync(CancellationToken cancellationToken = default)
        {
            (string Descricao, string Abreviatura)[] unidades =
            [
                ("Quilowatt-hora", "kWh"),
                ("Metro cubico", "m3"),
                ("Unidade", "un")
            ];

            foreach (var (descricao, abreviatura) in unidades)
            {
                await UnidadeMedidaFactory.New()
                    .State(u =>
                    {
                        u.Descricao = descricao;
                        u.Abreviatura = abreviatura;
                    })
                    .CreateAsync(Context, cancellationToken);
            }
        }
    }
}
