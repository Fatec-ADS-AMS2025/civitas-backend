using Civitas.WebAPI.Data.Factories;

namespace Civitas.WebAPI.Data.Seeders
{
    /// <summary>Cria os tipos de instituição.</summary>
    public class TipoInstituicaoSeeder : Seeder
    {
        public const string Escola = "Escola Municipal";
        public const string Saude = "Unidade Basica de Saude";

        public TipoInstituicaoSeeder(AppDbContext context, IServiceProvider services) : base(context, services) { }

        public override async Task RunAsync(CancellationToken cancellationToken = default)
        {
            foreach (var descricao in new[] { Escola, Saude, "Centro de Educacao Infantil" })
            {
                await TipoInstituicaoFactory.New()
                    .State(t => t.Descricao = descricao)
                    .CreateAsync(Context, cancellationToken);
            }
        }
    }
}
