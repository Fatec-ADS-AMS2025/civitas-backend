using Civitas.WebAPI.Data.Factories;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Seeders
{
    /// <summary>
    /// Cria uma unidade consumidora por orçamento, atendida por um fornecedor do mesmo tipo de despesa.
    /// Depende de <see cref="OrcamentoSeeder"/> e <see cref="FornecedorSeeder"/>.
    /// </summary>
    public class UnidadeConsumidoraSeeder : Seeder
    {
        public const string UcEscola = "UC-998877";
        public const string UcUbs = "UC-776655";

        public UnidadeConsumidoraSeeder(AppDbContext context, IServiceProvider services) : base(context, services) { }

        public override async Task RunAsync(CancellationToken cancellationToken = default)
        {
            var orcamentos = await Context.Orcamentos
                .Include(o => o.Instituicao)
                .Include(o => o.TipoDespesa)
                .OrderBy(o => o.IdOrcamento)
                .ToListAsync(cancellationToken);

            var fornecedores = await Context.Fornecedores.OrderBy(f => f.IdFornecedor).ToListAsync(cancellationToken);

            foreach (var orcamento in orcamentos)
            {
                var fornecedor = fornecedores.First(f => f.IdTipoDespesa == orcamento.IdTipoDespesa);
                var factory = UnidadeConsumidoraFactory.New().ParaOrcamento(orcamento).ParaFornecedor(fornecedor);

                if (orcamento.Instituicao!.Email == InstituicaoSeeder.EscolaEmail)
                {
                    factory.ComIdentificador(UcEscola);
                }
                else if (orcamento.Instituicao.Email == InstituicaoSeeder.UbsEmail)
                {
                    factory.ComIdentificador(UcUbs);
                }

                await factory.CreateAsync(Context, cancellationToken);
            }
        }
    }
}
