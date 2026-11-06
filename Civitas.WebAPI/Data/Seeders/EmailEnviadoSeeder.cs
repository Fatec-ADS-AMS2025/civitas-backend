using Civitas.WebAPI.Data.Factories;

namespace Civitas.WebAPI.Data.Seeders
{
    /// <summary>Cria um histórico de e-mails enviados, pendentes e com falha.</summary>
    public class EmailEnviadoSeeder : Seeder
    {
        public EmailEnviadoSeeder(AppDbContext context, IServiceProvider services) : base(context, services) { }

        public override async Task RunAsync(CancellationToken cancellationToken = default)
        {
            await EmailEnviadoFactory.New().Enviado().CreateAsync(Context, 5, cancellationToken);
            await EmailEnviadoFactory.New().Pendente().CreateAsync(Context, cancellationToken);
            await EmailEnviadoFactory.New().ComFalha().CreateAsync(Context, 2, cancellationToken);
        }
    }
}
