using Civitas.WebAPI.Data.Factories;

namespace Civitas.WebAPI.Data.Seeders
{
    /// <summary>Cria as secretarias de Educação e Saúde, mais secretarias aleatórias.</summary>
    public class SecretariaSeeder : Seeder
    {
        public const string EducacaoEmail = "educacao@civitas.dev";
        public const string SaudeEmail = "saude@civitas.dev";

        public SecretariaSeeder(AppDbContext context, IServiceProvider services) : base(context, services) { }

        public override async Task RunAsync(CancellationToken cancellationToken = default)
        {
            await SecretariaFactory.New()
                .State(s =>
                {
                    s.Nome = "Secretaria Municipal de Educacao";
                    s.NomeRazaoSocial = "Secretaria Municipal de Educacao de Curitiba";
                    s.Descricao = "Responsavel pela gestao da rede municipal de ensino.";
                    s.Cnpj = "11222333000144";
                    s.Email = EducacaoEmail;
                    s.Cidade = "Curitiba";
                    s.Estado = "PR";
                })
                .CreateAsync(Context, cancellationToken);

            await SecretariaFactory.New()
                .State(s =>
                {
                    s.Nome = "Secretaria Municipal de Saude";
                    s.NomeRazaoSocial = "Secretaria Municipal de Saude de Curitiba";
                    s.Descricao = "Responsavel pela gestao das unidades de atencao primaria.";
                    s.Cnpj = "22333444000155";
                    s.Email = SaudeEmail;
                    s.Cidade = "Curitiba";
                    s.Estado = "PR";
                })
                .CreateAsync(Context, cancellationToken);

            await SecretariaFactory.New().CreateAsync(Context, 2, cancellationToken);
        }
    }
}
