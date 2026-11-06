using Civitas.WebAPI.Data.Factories;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Seeders
{
    /// <summary>
    /// Cria a escola e a UBS de desenvolvimento, mais instituições aleatórias para cada secretaria.
    /// Depende de <see cref="SecretariaSeeder"/> e <see cref="TipoInstituicaoSeeder"/>.
    /// </summary>
    public class InstituicaoSeeder : Seeder
    {
        public const string EscolaEmail = "horizonte@civitas.dev";
        public const string UbsEmail = "ubs-aurora@civitas.dev";

        public InstituicaoSeeder(AppDbContext context, IServiceProvider services) : base(context, services) { }

        public override async Task RunAsync(CancellationToken cancellationToken = default)
        {
            var educacao = await Context.Secretarias.SingleAsync(s => s.Email == SecretariaSeeder.EducacaoEmail, cancellationToken);
            var saude = await Context.Secretarias.SingleAsync(s => s.Email == SecretariaSeeder.SaudeEmail, cancellationToken);
            var escola = await Context.TipoInstituicoes.SingleAsync(t => t.Descricao == TipoInstituicaoSeeder.Escola, cancellationToken);
            var ubs = await Context.TipoInstituicoes.SingleAsync(t => t.Descricao == TipoInstituicaoSeeder.Saude, cancellationToken);

            await InstituicaoFactory.New()
                .ParaSecretaria(educacao).ParaTipoInstituicao(escola)
                .State(i =>
                {
                    i.Nome = "Escola Municipal Horizonte";
                    i.NomeRazaoSocial = "Escola Municipal Horizonte";
                    i.CNPJ = "55666777000188";
                    i.Email = EscolaEmail;
                    i.Cidade = "Curitiba";
                    i.Estado = "PR";
                })
                .CreateAsync(Context, cancellationToken);

            await InstituicaoFactory.New()
                .ParaSecretaria(saude).ParaTipoInstituicao(ubs)
                .State(i =>
                {
                    i.Nome = "UBS Vila Aurora";
                    i.NomeRazaoSocial = "UBS Vila Aurora";
                    i.CNPJ = "66777888000199";
                    i.Email = UbsEmail;
                    i.Cidade = "Curitiba";
                    i.Estado = "PR";
                })
                .CreateAsync(Context, cancellationToken);

            var tipos = await Context.TipoInstituicoes.ToListAsync(cancellationToken);
            var faker = new Bogus.Faker("pt_BR");

            foreach (var secretaria in await Context.Secretarias.ToListAsync(cancellationToken))
            {
                await InstituicaoFactory.New()
                    .ParaSecretaria(secretaria)
                    .ParaTipoInstituicao(faker.PickRandom(tipos))
                    .CreateAsync(Context, cancellationToken);
            }
        }
    }
}
