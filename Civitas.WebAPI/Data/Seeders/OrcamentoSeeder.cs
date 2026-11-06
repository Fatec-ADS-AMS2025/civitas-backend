using Civitas.WebAPI.Data.Factories;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Seeders
{
    /// <summary>
    /// Cria um orçamento do ano corrente para cada instituição.
    /// Depende de <see cref="InstituicaoSeeder"/> e <see cref="TipoDespesaSeeder"/>.
    /// </summary>
    public class OrcamentoSeeder : Seeder
    {
        public OrcamentoSeeder(AppDbContext context, IServiceProvider services) : base(context, services) { }

        public override async Task RunAsync(CancellationToken cancellationToken = default)
        {
            var energia = await Context.TiposDespesa.SingleAsync(t => t.Descricao == TipoDespesaSeeder.Energia, cancellationToken);
            var manutencao = await Context.TiposDespesa.SingleAsync(t => t.Descricao == TipoDespesaSeeder.Manutencao, cancellationToken);
            var escola = await Context.Instituicoes.SingleAsync(i => i.Email == InstituicaoSeeder.EscolaEmail, cancellationToken);
            var ubs = await Context.Instituicoes.SingleAsync(i => i.Email == InstituicaoSeeder.UbsEmail, cancellationToken);

            await OrcamentoFactory.New()
                .ParaInstituicao(escola).ParaTipoDespesa(energia).ComValor(350_000m)
                .CreateAsync(Context, cancellationToken);

            await OrcamentoFactory.New()
                .ParaInstituicao(ubs).ParaTipoDespesa(manutencao).ComValor(420_000m)
                .CreateAsync(Context, cancellationToken);

            var tipos = await Context.TiposDespesa.ToListAsync(cancellationToken);
            var outras = await Context.Instituicoes
                .Where(i => i.Id != escola.Id && i.Id != ubs.Id)
                .ToListAsync(cancellationToken);
            var faker = new Bogus.Faker("pt_BR");

            foreach (var instituicao in outras)
            {
                await OrcamentoFactory.New()
                    .ParaInstituicao(instituicao)
                    .ParaTipoDespesa(faker.PickRandom(tipos))
                    .CreateAsync(Context, cancellationToken);
            }
        }
    }
}
