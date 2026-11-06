using Civitas.WebAPI.Data.Factories;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Seeders
{
    /// <summary>Cria fornecedores para cada tipo de despesa. Depende de <see cref="TipoDespesaSeeder"/>.</summary>
    public class FornecedorSeeder : Seeder
    {
        public FornecedorSeeder(AppDbContext context, IServiceProvider services) : base(context, services) { }

        public override async Task RunAsync(CancellationToken cancellationToken = default)
        {
            var energia = await Context.TiposDespesa.SingleAsync(t => t.Descricao == TipoDespesaSeeder.Energia, cancellationToken);
            var manutencao = await Context.TiposDespesa.SingleAsync(t => t.Descricao == TipoDespesaSeeder.Manutencao, cancellationToken);

            await FornecedorFactory.New()
                .ParaTipoDespesa(energia)
                .State(f =>
                {
                    f.NomeFantasia = "Copel Dev";
                    f.Nome = "Companhia Paranaense de Energia Dev";
                    f.Cnpj = "33444555000166";
                    f.Email = "faturamento@copel.dev";
                })
                .CreateAsync(Context, cancellationToken);

            await FornecedorFactory.New()
                .ParaTipoDespesa(manutencao)
                .State(f =>
                {
                    f.NomeFantasia = "Manutencao Escolar Dev";
                    f.Nome = "Manutencao Escolar LTDA";
                    f.Cnpj = "44555666000177";
                    f.Email = "contato@manutencao.dev";
                })
                .CreateAsync(Context, cancellationToken);

            // Um fornecedor aleatório extra para cada tipo de despesa.
            foreach (var tipoDespesa in await Context.TiposDespesa.ToListAsync(cancellationToken))
            {
                await FornecedorFactory.New().ParaTipoDespesa(tipoDespesa).CreateAsync(Context, cancellationToken);
            }
        }
    }
}
