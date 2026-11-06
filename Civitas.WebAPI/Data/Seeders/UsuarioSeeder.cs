using Civitas.WebAPI.Data.Factories;

namespace Civitas.WebAPI.Data.Seeders
{
    /// <summary>Cria o administrador e o funcionário de desenvolvimento, mais funcionários aleatórios.</summary>
    public class UsuarioSeeder : Seeder
    {
        public const string AdminEmail = "admin@civitas.dev";
        public const string FuncionarioEmail = "funcionario@civitas.dev";

        public UsuarioSeeder(AppDbContext context, IServiceProvider services) : base(context, services) { }

        public override async Task RunAsync(CancellationToken cancellationToken = default)
        {
            await UsuarioFactory.New()
                .Administrador()
                .State(u =>
                {
                    u.Nome = "Administrador Dev";
                    u.Cpf = "12345678901";
                    u.Email = AdminEmail;
                    u.Matricula = "ADM-0001";
                })
                .CreateAsync(Context, cancellationToken);

            await UsuarioFactory.New()
                .Funcionario()
                .State(u =>
                {
                    u.Nome = "Funcionario Dev";
                    u.Cpf = "98765432100";
                    u.Email = FuncionarioEmail;
                    u.Matricula = "FUN-0001";
                })
                .CreateAsync(Context, cancellationToken);

            await UsuarioFactory.New().Funcionario().CreateAsync(Context, 5, cancellationToken);
            await UsuarioFactory.New().Funcionario().Inativo().CreateAsync(Context, cancellationToken);
        }
    }
}
