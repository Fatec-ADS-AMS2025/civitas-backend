using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Civitas.WebAPI.Controllers;
using Civitas.WebAPI.Data;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Services.Interfaces;
using Civitas.WebAPI.Tests.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Civitas.WebAPI.Tests;

public sealed class PasswordRecoveryEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public PasswordRecoveryEndpointsTests(TestWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task Register_WithValidPayload_CreatesActiveVisitorAndReturnsSafeResponse()
    {
        await using var harness = await CreateHarnessAsync();
        using var client = harness.Factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/register", Registration());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("senha", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("hash", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("tipousuario", body, StringComparison.OrdinalIgnoreCase);

        await using var scope = harness.Factory.Services.CreateAsyncScope();
        var usuario = Assert.Single(await scope.ServiceProvider.GetRequiredService<AppDbContext>().Usuarios.ToListAsync());
        Assert.Equal(TipoUsuario.VISITANTE, usuario.TipoUsuario);
        Assert.Equal(Situacao.ATIVO, usuario.Situacao);
        Assert.NotEqual("Senha123", usuario.Senha);
    }

    [Fact]
    public async Task Register_WithInvalidPayload_ReturnsValidationEnvelope()
    {
        await using var harness = await CreateHarnessAsync();
        using var client = harness.Factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/register", new { nome = "A", cpf = "123", senha = "fraca" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("inválidos", await response.Content.ReadAsStringAsync(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Register_EnforcesCpfEmailAndMatriculaUniqueness_AndKeepsRgNonUniqueByExistingRule()
    {
        await using var harness = await CreateHarnessAsync();
        using var client = harness.Factory.CreateClient();
        Assert.Equal(HttpStatusCode.Created, (await client.PostAsJsonAsync("/api/auth/register", Registration())).StatusCode);

        Assert.Equal(HttpStatusCode.Conflict, (await client.PostAsJsonAsync("/api/auth/register", Registration(email: "cpf@example.com", matricula: "VIS-002"))).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await client.PostAsJsonAsync("/api/auth/register", Registration(cpf: "11144477735", matricula: "VIS-003"))).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await client.PostAsJsonAsync("/api/auth/register", Registration(cpf: "93541134780", email: "matricula@example.com"))).StatusCode);

        var repeatedRg = await client.PostAsJsonAsync("/api/auth/register", Registration(cpf: "12345678909", email: "rg@example.com", matricula: "VIS-004"));
        Assert.Equal(HttpStatusCode.Created, repeatedRg.StatusCode);
    }

    [Fact]
    public async Task Register_IgnoresAttemptToChooseAdministrativeProfile()
    {
        await using var harness = await CreateHarnessAsync();
        using var client = harness.Factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/register", Registration(tipoUsuario: (int)TipoUsuario.ADMINISTRADOR));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        await using var scope = harness.Factory.Services.CreateAsyncScope();
        var usuario = Assert.Single(await scope.ServiceProvider.GetRequiredService<AppDbContext>().Usuarios.ToListAsync());
        Assert.Equal(TipoUsuario.VISITANTE, usuario.TipoUsuario);
    }

    [Fact]
    public async Task ForgotPassword_ForKnownAndUnknownEmails_ReturnsIdenticalEnvelopeAndPersistsNoRawToken()
    {
        await using var harness = await CreateHarnessAsync();
        using var client = harness.Factory.CreateClient();
        await RegisterAsync(client);

        var known = await client.PostAsJsonAsync("/api/auth/forgot-password", new { email = " ANA@EXAMPLE.COM " });
        var unknown = await client.PostAsJsonAsync("/api/auth/forgot-password", new { email = "ausente@example.com" });

        Assert.Equal(HttpStatusCode.OK, known.StatusCode);
        Assert.Equal(await known.Content.ReadAsStringAsync(), await unknown.Content.ReadAsStringAsync());
        var rawToken = ExtractToken(harness.Sender.Content);

        await using var scope = harness.Factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var token = Assert.Single(await context.PasswordResetTokens.ToListAsync());
        Assert.Equal(64, token.TokenHash.Length);
        Assert.NotEqual(rawToken, token.TokenHash);
        Assert.DoesNotContain(rawToken, token.TokenHash, StringComparison.Ordinal);
        Assert.InRange(token.ExpiraEm, DateTime.UtcNow.AddMinutes(59), DateTime.UtcNow.AddMinutes(61));
        Assert.Empty(await context.EmailsEnviados.ToListAsync());
    }

    [Fact]
    public async Task ForgotPassword_WhenSmtpFails_InvalidatesTokenAndDoesNotExposeDetails()
    {
        await using var harness = await CreateHarnessAsync(senderFails: true);
        using var client = harness.Factory.CreateClient();
        await RegisterAsync(client);

        var response = await client.PostAsJsonAsync("/api/auth/forgot-password", new { email = "ana@example.com" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("smtp", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("ana@example.com", body, StringComparison.OrdinalIgnoreCase);
        await using var scope = harness.Factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.NotNull(Assert.Single(await context.PasswordResetTokens.ToListAsync()).UtilizadoEm);
        Assert.Empty(await context.EmailsEnviados.ToListAsync());
    }

    [Fact]
    public async Task ForgotPassword_InvalidatesPreviouslyUnusedToken()
    {
        await using var harness = await CreateHarnessAsync();
        using var client = harness.Factory.CreateClient();
        await RegisterAsync(client);
        var firstToken = await RequestRecoveryAsync(client, harness.Sender);
        var secondToken = await RequestRecoveryAsync(client, harness.Sender);

        Assert.NotEqual(firstToken, secondToken);
        await using (var scope = harness.Factory.Services.CreateAsyncScope())
        {
            var tokens = await scope.ServiceProvider.GetRequiredService<AppDbContext>().PasswordResetTokens
                .OrderBy(token => token.Id)
                .ToListAsync();
            Assert.Equal(2, tokens.Count);
            Assert.NotNull(tokens[0].UtilizadoEm);
            Assert.Null(tokens[1].UtilizadoEm);
        }

        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/api/auth/reset-password", new { token = firstToken, senha = "NovaSenha123" })).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsJsonAsync("/api/auth/reset-password", new { token = secondToken, senha = "NovaSenha123" })).StatusCode);
    }

    [Fact]
    public async Task ResetPassword_WithInvalidToken_ReturnsSafeBadRequest()
    {
        await using var harness = await CreateHarnessAsync();
        using var client = harness.Factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/reset-password", new { token = new string('A', 64), senha = "NovaSenha123" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("inválido", await response.Content.ReadAsStringAsync(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ResetPassword_WithExpiredToken_ReturnsSafeBadRequest()
    {
        await using var harness = await CreateHarnessAsync();
        using var client = harness.Factory.CreateClient();
        await RegisterAsync(client);
        var rawToken = await RequestRecoveryAsync(client, harness.Sender);
        await ExpireTokenAsync(harness.Factory);

        var response = await client.PostAsJsonAsync("/api/auth/reset-password", new { token = rawToken, senha = "NovaSenha123" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ResetPassword_WithInvalidPassword_DoesNotConsumeToken()
    {
        await using var harness = await CreateHarnessAsync();
        using var client = harness.Factory.CreateClient();
        await RegisterAsync(client);
        var rawToken = await RequestRecoveryAsync(client, harness.Sender);

        var invalid = await client.PostAsJsonAsync("/api/auth/reset-password", new { token = rawToken, senha = "fraca" });
        var valid = await client.PostAsJsonAsync("/api/auth/reset-password", new { token = rawToken, senha = "NovaSenha123" });

        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
        Assert.Equal(HttpStatusCode.OK, valid.StatusCode);
    }

    [Fact]
    public async Task ResetPassword_WithValidToken_ChangesLoginAndPreventsReuse()
    {
        await using var harness = await CreateHarnessAsync();
        using var client = harness.Factory.CreateClient();
        await RegisterAsync(client);
        var rawToken = await RequestRecoveryAsync(client, harness.Sender);

        Assert.Equal(HttpStatusCode.OK, (await client.PostAsJsonAsync("/api/auth/reset-password", new { token = rawToken, senha = "NovaSenha123" })).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/api/auth/reset-password", new { token = rawToken, senha = "OutraSenha123" })).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsJsonAsync("/api/auth/login", new { email = "ana@example.com", senha = "NovaSenha123" })).StatusCode);
    }

    private async Task<TestHarness> CreateHarnessAsync(bool senderFails = false)
    {
        var sender = new CapturingEmailSender { ShouldFail = senderFails };
        var factory = _factory.WithWebHostBuilder(builder => builder.ConfigureServices(services =>
        {
            services.RemoveAll<IEmailSender>();
            services.AddSingleton<IEmailSender>(sender);
        }));
        await ResetDatabaseAsync(factory);
        return new TestHarness(factory, sender);
    }

    private static async Task ResetDatabaseAsync(WebApplicationFactory<FornecedorController> factory)
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    private static async Task RegisterAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/auth/register", Registration());
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private static async Task<string> RequestRecoveryAsync(HttpClient client, CapturingEmailSender sender)
    {
        var response = await client.PostAsJsonAsync("/api/auth/forgot-password", new { email = "ana@example.com" });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return ExtractToken(sender.Content);
    }

    private static async Task ExpireTokenAsync(WebApplicationFactory<FornecedorController> factory)
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var token = Assert.Single(await context.PasswordResetTokens.ToListAsync());
        token.ExpiraEm = DateTime.UtcNow.AddMinutes(-1);
        await context.SaveChangesAsync();
    }

    private static string ExtractToken(string? content)
    {
        var token = Regex.Match(content ?? string.Empty, "token=([A-F0-9]+)").Groups[1].Value;
        Assert.NotEmpty(token);
        return token;
    }

    private static object Registration(
        string cpf = "52998224725",
        string email = "ana@example.com",
        string matricula = "VIS-001",
        int? tipoUsuario = null) => new
    {
        nome = "Ana da Silva", cpf, rg = "12345678", logradouro = "Rua das Flores", numero = "123",
        bairro = "Centro", cidade = "Maringá", estado = "PR", cep = "87060000", email, senha = "Senha123", matricula, tipoUsuario
    };

    private sealed class CapturingEmailSender : IEmailSender
    {
        public bool ShouldFail { get; init; }
        public string? Content { get; private set; }

        public Task SendAsync(string destinatario, string assunto, string conteudo, CancellationToken cancellationToken = default)
        {
            Content = conteudo;
            if (ShouldFail) throw new InvalidOperationException("SMTP unavailable");
            return Task.CompletedTask;
        }
    }

    private sealed class TestHarness : IAsyncDisposable
    {
        public TestHarness(WebApplicationFactory<FornecedorController> factory, CapturingEmailSender sender)
        {
            Factory = factory;
            Sender = sender;
        }

        public WebApplicationFactory<FornecedorController> Factory { get; }
        public CapturingEmailSender Sender { get; }
        public ValueTask DisposeAsync() => Factory.DisposeAsync();
    }
}
