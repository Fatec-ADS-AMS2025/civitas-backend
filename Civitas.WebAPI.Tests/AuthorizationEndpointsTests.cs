using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Tests.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace Civitas.WebAPI.Tests;

/// <summary>
/// Testes de autorização JWT.
/// Verifica que rotas protegidas exigem token válido (DEV=false)
/// e que ficam acessíveis sem token quando DEV=true.
/// </summary>
public sealed class AuthorizationEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private const string JwtSecret = "development-only-key-change-before-production-2026";
    private const string JwtIssuer = "Civitas.WebAPI";
    private const string JwtAudience = "Civitas.Client";

    // Rota protegida usada como referência nos testes
    private const string ProtectedRoute = "/api/usuarios";

    private readonly TestWebApplicationFactory _factory;

    public AuthorizationEndpointsTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    // ── Critério 1: login funciona sem autenticação ──────────────────────────

    [Fact]
    public async Task PostLogin_WithValidCredentials_WithoutAuthHeader_Returns200()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            context.Usuarios.Add(CreateUsuario(
                id: 1,
                email: "public@example.com",
                senha: BCrypt.Net.BCrypt.HashPassword("abc12345")));
            return Task.CompletedTask;
        });

        using var client = _factory.CreateClient();
        // Sem header Authorization — endpoint deve ser público
        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "public@example.com",
            senha = "abc12345"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // ── Critério 2: endpoint protegido retorna 401 sem token (DEV=false) ─────

    [Fact]
    public async Task GetProtectedRoute_WithoutToken_Returns401_WhenDevFalse()
    {
        await _factory.ResetDatabaseAsync(_ => Task.CompletedTask);
        using var client = _factory.CreateClient();

        var response = await client.GetAsync(ProtectedRoute);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // ── Critério 3: endpoint protegido retorna 401 com token inválido ────────

    [Fact]
    public async Task GetProtectedRoute_WithInvalidToken_Returns401_WhenDevFalse()
    {
        await _factory.ResetDatabaseAsync(_ => Task.CompletedTask);
        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "token.invalido.aqui");

        var response = await client.GetAsync(ProtectedRoute);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // ── Critério 4: endpoint protegido retorna 401 com token expirado ────────

    [Fact]
    public async Task GetProtectedRoute_WithExpiredToken_Returns401_WhenDevFalse()
    {
        await _factory.ResetDatabaseAsync(_ => Task.CompletedTask);
        using var client = _factory.CreateClient();

        var expiredToken = BuildToken(expiresAt: DateTime.UtcNow.AddMinutes(-5));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", expiredToken);

        var response = await client.GetAsync(ProtectedRoute);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // ── Critério 5: endpoint protegido retorna sucesso com token válido ──────

    [Fact]
    public async Task GetProtectedRoute_WithValidToken_ReturnsSuccess_WhenDevFalse()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            context.Usuarios.Add(CreateUsuario(
                id: 1,
                email: "admin@example.com",
                senha: BCrypt.Net.BCrypt.HashPassword("abc12345")));
            return Task.CompletedTask;
        });

        using var client = _factory.CreateClient();

        // Obtém token via login
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "admin@example.com",
            senha = "abc12345"
        });
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var body = await loginResponse.Content.ReadFromJsonAsync<LoginEnvelope>(
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(body?.Data?.Token);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", body!.Data!.Token);

        var response = await client.GetAsync(ProtectedRoute);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // ── Critério 6: endpoint protegido acessível sem token (DEV=true) ────────

    [Fact]
    public async Task GetProtectedRoute_WithoutToken_ReturnsSuccess_WhenDevTrue()
    {
        await using var devFactory = new DevModeFactory();
        await devFactory.InitializeAsync();
        await devFactory.ResetDatabaseAsync(_ => Task.CompletedTask);

        using var client = devFactory.CreateClient();
        var response = await client.GetAsync(ProtectedRoute);

        // Sem token mas DEV=true → deve passar pela autorização (200 ou outro código de app)
        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // ── Critério 7: token com claims adicionais ───────────────────────────────

    [Fact]
    public async Task GetProtectedRoute_WithTokenContainingExtraClaims_ReturnsSuccess()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            context.Usuarios.Add(CreateUsuario(
                id: 1,
                email: "admin@example.com",
                senha: BCrypt.Net.BCrypt.HashPassword("abc12345")));
            return Task.CompletedTask;
        });

        using var client = _factory.CreateClient();

        // Obtém token padrão via login e verifica que endpoint aceita
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "admin@example.com",
            senha = "abc12345"
        });
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var body = await loginResponse.Content.ReadFromJsonAsync<LoginEnvelope>(
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(body?.Data?.Token);

        // Gera token customizado com claims adicionais usando a mesma chave
        var tokenComExtraClaims = BuildToken(
            expiresAt: DateTime.UtcNow.AddHours(1),
            extraClaims: new[] { new Claim("role", "gestor"), new Claim("setor", "financeiro") });

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenComExtraClaims);

        var response = await client.GetAsync(ProtectedRoute);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static string BuildToken(DateTime expiresAt, IEnumerable<Claim>? extraClaims = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, "1"),
            new Claim("name", "Teste"),
            new Claim("tipousuario", "ADMINISTRADOR")
        };

        if (extraClaims != null)
            claims.AddRange(extraClaims);

        var token = new JwtSecurityToken(
            issuer: JwtIssuer,
            audience: JwtAudience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static Usuario CreateUsuario(int id, string email, string senha)
    {
        return new Usuario(
            id,
            "52998224725",
            "Admin Teste",
            "12345678",
            "Rua das Flores",
            "123",
            "Maringa",
            "PR",
            "87060000",
            email,
            senha,
            Situacao.ATIVO,
            "MAT-001",
            TipoUsuario.ADMINISTRADOR,
            "Centro");
    }

    private sealed record LoginEnvelope(int Code, string? Message, LoginData? Data);
    private sealed record LoginData(string Token, string ExpiresAtUtc, LoginUsuario Usuario);
    private sealed record LoginUsuario(int Id, string Nome, string TipoUsuario);

    // Factory independente que habilita DEV=true e substitui PostgreSQL por SQLite
    private sealed class DevModeFactory : WebApplicationFactory<Civitas.WebAPI.Controllers.AuthController>, IAsyncLifetime
    {
        private readonly Microsoft.Data.Sqlite.SqliteConnection _connection = new("Data Source=:memory:");

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["DEV"] = "true"
                });
            });

            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<Civitas.WebAPI.Data.AppDbContext>));
                services.RemoveAll(typeof(IDbContextOptionsConfiguration<Civitas.WebAPI.Data.AppDbContext>));
                services.RemoveAll<Civitas.WebAPI.Data.AppDbContext>();

                services.AddDbContext<Civitas.WebAPI.Data.AppDbContext>(options =>
                    options.UseSqlite(_connection));
            });
        }

        public async Task InitializeAsync() => await _connection.OpenAsync();

        public new async Task DisposeAsync() => await _connection.DisposeAsync();

        public async Task ResetDatabaseAsync(Func<Civitas.WebAPI.Data.AppDbContext, Task> seedAsync)
        {
            using var scope = Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<Civitas.WebAPI.Data.AppDbContext>();
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            await seedAsync(context);
            await context.SaveChangesAsync();
        }
    }
}
