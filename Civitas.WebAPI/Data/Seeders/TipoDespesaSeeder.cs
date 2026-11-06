using Civitas.WebAPI.Data.Factories;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Seeders
{
    /// <summary>Cria os tipos de despesa. Depende de <see cref="TipoCodigoSeeder"/> e <see cref="UnidadeMedidaSeeder"/>.</summary>
    public class TipoDespesaSeeder : Seeder
    {
        public const string Energia = "Energia Eletrica";
        public const string Agua = "Agua e Esgoto";
        public const string Manutencao = "Manutencao Predial";

        public TipoDespesaSeeder(AppDbContext context, IServiceProvider services) : base(context, services) { }

        public override async Task RunAsync(CancellationToken cancellationToken = default)
        {
            var consumo = await Context.TipoCodigos.SingleAsync(t => t.Nome == TipoCodigoSeeder.Consumo, cancellationToken);
            var servico = await Context.TipoCodigos.SingleAsync(t => t.Nome == TipoCodigoSeeder.Servico, cancellationToken);
            var kwh = await Context.UnidadesMedida.SingleAsync(u => u.Abreviatura == "kWh", cancellationToken);
            var m3 = await Context.UnidadesMedida.SingleAsync(u => u.Abreviatura == "m3", cancellationToken);
            var un = await Context.UnidadesMedida.SingleAsync(u => u.Abreviatura == "un", cancellationToken);

            await TipoDespesaFactory.New()
                .ParaTipoCodigo(consumo).ParaUnidadeMedida(kwh).SolicitaUc()
                .State(t => t.Descricao = Energia)
                .CreateAsync(Context, cancellationToken);

            await TipoDespesaFactory.New()
                .ParaTipoCodigo(consumo).ParaUnidadeMedida(m3).SolicitaUc()
                .ComCamposOpcionais("numeroHidrometro", "leituraAnterior")
                .State(t => t.Descricao = Agua)
                .CreateAsync(Context, cancellationToken);

            await TipoDespesaFactory.New()
                .ParaTipoCodigo(servico).ParaUnidadeMedida(un).NaoSolicitaUc()
                .State(t => t.Descricao = Manutencao)
                .CreateAsync(Context, cancellationToken);
        }
    }
}
