using Civitas.WebAPI.Data.Factories;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Seeders
{
    /// <summary>Registra a criação das despesas seedadas na auditoria. Depende de <see cref="DespesaSeeder"/>.</summary>
    public class AuditoriaSeeder : Seeder
    {
        public AuditoriaSeeder(AppDbContext context, IServiceProvider services) : base(context, services) { }

        public override async Task RunAsync(CancellationToken cancellationToken = default)
        {
            var despesas = await Context.Despesas
                .Include(d => d.Usuario)
                .OrderBy(d => d.Id)
                .ToListAsync(cancellationToken);

            foreach (var despesa in despesas)
            {
                await AuditoriaFactory.New()
                    .ParaUsuario(despesa.Usuario)
                    .Insercao("Despesa", new { numeroDocumento = despesa.NumeroDocumento, status = despesa.Status.ToString() })
                    .CreateAsync(Context, cancellationToken);
            }
        }
    }
}
