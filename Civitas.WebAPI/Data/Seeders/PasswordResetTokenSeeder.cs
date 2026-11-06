using Civitas.WebAPI.Data.Factories;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Seeders
{
    /// <summary>
    /// Cria tokens de recuperação já utilizados/expirados (histórico) para o funcionário de desenvolvimento.
    /// Não cria token ativo: o fluxo real de "esqueci minha senha" deve gerar o seu.
    /// </summary>
    public class PasswordResetTokenSeeder : Seeder
    {
        public PasswordResetTokenSeeder(AppDbContext context, IServiceProvider services) : base(context, services) { }

        public override async Task RunAsync(CancellationToken cancellationToken = default)
        {
            var funcionario = await Context.Usuarios.SingleAsync(u => u.Email == UsuarioSeeder.FuncionarioEmail, cancellationToken);

            await PasswordResetTokenFactory.New().ParaUsuario(funcionario).Expirado().Utilizado()
                .CreateAsync(Context, cancellationToken);

            await PasswordResetTokenFactory.New().ParaUsuario(funcionario).Expirado().Utilizado()
                .CreateAsync(Context, cancellationToken);
        }
    }
}
