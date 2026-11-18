using Civitas.WebAPI.Controllers;
using Civitas.WebAPI.Data;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Civitas.WebAPI.Tests.Infrastructure;

public sealed class TestWebApplicationFactory : WebApplicationFactory<FornecedorController>, IAsyncLifetime
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging => logging.ClearProviders());
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
            services.RemoveAll(typeof(IDbContextOptionsConfiguration<AppDbContext>));
            services.RemoveAll<AppDbContext>();

            services.AddDbContext<AppDbContext>(options => options.UseSqlite(_connection));
        });
    }

    public async Task InitializeAsync()
    {
        await _connection.OpenAsync();
    }

    public new async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
    }

    public async Task ResetDatabaseAsync(Func<AppDbContext, Task> seedAsync)
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        await seedAsync(context);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Recria o banco já com o usuário 1, dono do token usado pelos testes autenticados.
    /// Toda escrita gera auditoria com o autor do token, e a auditoria exige um usuário existente.
    /// </summary>
    public Task ResetDatabaseWithAuthenticatedUserAsync(Func<AppDbContext, Task> seedAsync) =>
        ResetDatabaseAsync(async context =>
        {
            context.Usuarios.Add(new Usuario(
                1, "11144477735", "Usuario Autenticado", "123456789", "Rua Teste", "1",
                "Maringa", "PR", "87000000", "autenticado@example.com", "hash", Situacao.ATIVO,
                "MAT-AUTH", TipoUsuario.ADMINISTRADOR, "Centro"));
            await seedAsync(context);
        });
}
