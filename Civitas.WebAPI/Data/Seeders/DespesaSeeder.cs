using Civitas.WebAPI.Data.Factories;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Seeders
{
    /// <summary>
    /// Cria despesas pagas, a pagar e atrasadas para cada unidade consumidora.
    /// Depende de <see cref="UsuarioSeeder"/> e <see cref="UnidadeConsumidoraSeeder"/>.
    /// </summary>
    public class DespesaSeeder : Seeder
    {
        public DespesaSeeder(AppDbContext context, IServiceProvider services) : base(context, services) { }

        public override async Task RunAsync(CancellationToken cancellationToken = default)
        {
            var admin = await Context.Usuarios.SingleAsync(u => u.Email == UsuarioSeeder.AdminEmail, cancellationToken);
            var funcionario = await Context.Usuarios.SingleAsync(u => u.Email == UsuarioSeeder.FuncionarioEmail, cancellationToken);
            var unidades = await Context.UnidadesConsumidoras.ToListAsync(cancellationToken);

            foreach (var unidade in unidades)
            {
                await DespesaFactory.New().ParaUnidadeConsumidora(unidade).ParaUsuario(admin).Paga()
                    .CreateAsync(Context, 2, cancellationToken);

                await DespesaFactory.New().ParaUnidadeConsumidora(unidade).ParaUsuario(funcionario).APagar()
                    .CreateAsync(Context, cancellationToken);

                await DespesaFactory.New().ParaUnidadeConsumidora(unidade).ParaUsuario(funcionario).Atrasada()
                    .CreateAsync(Context, cancellationToken);
            }
        }
    }
}
