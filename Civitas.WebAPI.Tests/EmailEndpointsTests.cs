using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Civitas.WebAPI.Controllers;
using Civitas.WebAPI.Data;
using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Services.Interfaces;
using Civitas.WebAPI.Tests.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace Civitas.WebAPI.Tests;

public sealed class EmailEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly TestWebApplicationFactory _factory;

    public EmailEndpointsTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostEmail_WithEmptyRequiredFields_ReturnsAllErrorsWithoutStackTrace()
    {
        using var harness = await CreateHarness();
        using var client = CreateAuthenticatedClient(harness.Factory);

        var response = await client.PostAsJsonAsync("/api/email", CreatePayload("   ", "   ", "   "));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);
        var errors = ReadErrors(document);

        Assert.Equal((int)ResponseEnum.INVALID, document.RootElement.GetProperty("code").GetInt32());
        Assert.Contains(errors, error => error.Contains("EmailDestinatario", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(errors, error => error.Contains("Assunto", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(errors, error => error.Contains("Conteudo", StringComparison.OrdinalIgnoreCase)
            || error.Contains("Conteúdo", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain("StackTrace", body, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(0, harness.Sender.Calls);
    }

    [Theory]
    [InlineData("email-invalido")]
    [InlineData("usuario @dominio.com")]
    [InlineData("usuario@ dominio.com")]
    public async Task PostEmail_WithInvalidRecipient_ReturnsBadRequest(string email)
    {
        using var harness = await CreateHarness();
        using var client = CreateAuthenticatedClient(harness.Factory);

        var response = await client.PostAsJsonAsync("/api/email", CreatePayload(email));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("e-mail", body, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(0, harness.Sender.Calls);
    }

    [Fact]
    public async Task PostEmail_WithSubjectAboveLimit_ReturnsBadRequest()
    {
        using var harness = await CreateHarness();
        using var client = CreateAuthenticatedClient(harness.Factory);

        var response = await client.PostAsJsonAsync(
            "/api/email",
            CreatePayload(assunto: new string('A', 201)));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("200", await response.Content.ReadAsStringAsync());
        Assert.Equal(0, harness.Sender.Calls);
    }

    [Fact]
    public async Task PostEmail_WithContentAboveLimit_ReturnsBadRequest()
    {
        using var harness = await CreateHarness();
        using var client = CreateAuthenticatedClient(harness.Factory);

        var response = await client.PostAsJsonAsync(
            "/api/email",
            CreatePayload(conteudo: new string('B', 10001)));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("10000", await response.Content.ReadAsStringAsync());
        Assert.Equal(0, harness.Sender.Calls);
    }

    [Fact]
    public async Task PostEmail_WithValidPayload_SendsNormalizesAndPersistsEnviado()
    {
        using var harness = await CreateHarness();
        using var client = CreateAuthenticatedClient(harness.Factory);

        var response = await client.PostAsJsonAsync(
            "/api/email",
            CreatePayload("  USER@Example.COM  ", "  Aviso de consumo  ", "  Conteudo persistido  "));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var envelope = await response.Content.ReadFromJsonAsync<ResponseEnvelope<EmailData>>(JsonOptions);
        Assert.NotNull(envelope);
        Assert.Equal((int)ResponseEnum.SUCCESS, envelope!.Code);
        Assert.Equal("user@example.com", envelope.Data!.EmailDestinatario);
        Assert.Equal("Aviso de consumo", envelope.Data.Assunto);
        Assert.Equal("Conteudo persistido", envelope.Data.Conteudo);
        Assert.Equal((int)EmailStatusEnum.ENVIADO, envelope.Data.Status);
        Assert.Null(envelope.Data.MensagemErro);
        Assert.NotNull(envelope.Data.DataEnvio);
        Assert.NotEqual(default, envelope.Data.DataCriacao);

        await using var scope = harness.Factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var persisted = Assert.Single(context.EmailsEnviados);
        Assert.Equal("user@example.com", persisted.EmailDestinatario);
        Assert.Equal("Aviso de consumo", persisted.Assunto);
        Assert.Equal("Conteudo persistido", persisted.Conteudo);
        Assert.Equal(EmailStatusEnum.ENVIADO, persisted.Status);
        Assert.NotNull(persisted.DataEnvio);

        Assert.Equal(1, harness.Sender.Calls);
        Assert.Equal("user@example.com", harness.Sender.LastMessage!.Value.Destinatario);
    }

    [Fact]
    public async Task PostEmail_WhenProviderFails_PersistsFalhaAndReturnsClearError()
    {
        using var harness = await CreateHarness(senderFails: true);
        using var client = CreateAuthenticatedClient(harness.Factory);

        var response = await client.PostAsJsonAsync("/api/email", CreatePayload());

        Assert.Equal(HttpStatusCode.BadGateway, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);
        Assert.Equal((int)ResponseEnum.ERROR, document.RootElement.GetProperty("code").GetInt32());
        Assert.Contains("enviar", document.RootElement.GetProperty("message").GetString(), StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("StackTrace", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("smtp-indisponivel", body, StringComparison.OrdinalIgnoreCase);

        await using var scope = harness.Factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var persisted = Assert.Single(context.EmailsEnviados);
        Assert.Equal(EmailStatusEnum.FALHA, persisted.Status);
        Assert.False(string.IsNullOrWhiteSpace(persisted.MensagemErro));
        Assert.Equal("conteudo da mensagem", persisted.Conteudo);
    }

    [Fact]
    public async Task GetEmailById_WhenExists_ReturnsPersistedEmail()
    {
        using var harness = await CreateHarness();
        using var client = CreateAuthenticatedClient(harness.Factory);
        var created = await SendValidEmail(client, "destino@civitas.app");

        var response = await client.GetAsync($"/api/email/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var envelope = await response.Content.ReadFromJsonAsync<ResponseEnvelope<EmailData>>(JsonOptions);
        Assert.Equal(created.Id, envelope!.Data!.Id);
        Assert.Equal("destino@civitas.app", envelope.Data.EmailDestinatario);
    }

    [Fact]
    public async Task GetEmailById_WhenMissing_ReturnsNotFoundWithoutStackTrace()
    {
        using var harness = await CreateHarness();
        using var client = CreateAuthenticatedClient(harness.Factory);

        var response = await client.GetAsync("/api/email/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);
        Assert.Equal((int)ResponseEnum.NOT_FOUND, document.RootElement.GetProperty("code").GetInt32());
        Assert.Contains("não foi encontrado", document.RootElement.GetProperty("message").GetString(), StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("StackTrace", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetEmails_ReturnsPersistedHistory()
    {
        using var harness = await CreateHarness();
        using var client = CreateAuthenticatedClient(harness.Factory);
        await SendValidEmail(client, "a@civitas.app");
        await SendValidEmail(client, "b@civitas.app");

        var response = await client.GetAsync("/api/email");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var envelope = await response.Content.ReadFromJsonAsync<ResponseEnvelope<List<EmailData>>>(JsonOptions);
        Assert.Equal(2, envelope!.Data!.Count);
    }

    [Fact]
    public async Task GetEmailsByDestinatario_FiltersNormalizedEmail()
    {
        using var harness = await CreateHarness();
        using var client = CreateAuthenticatedClient(harness.Factory);
        await SendValidEmail(client, "alvo@civitas.app");
        await SendValidEmail(client, "outro@civitas.app");

        var response = await client.GetAsync("/api/email/destinatario/  ALVO@civitas.app  ");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var envelope = await response.Content.ReadFromJsonAsync<ResponseEnvelope<List<EmailData>>>(JsonOptions);
        var item = Assert.Single(envelope!.Data!);
        Assert.Equal("alvo@civitas.app", item.EmailDestinatario);
    }

    [Fact]
    public async Task GetEmailsByDestinatario_WithInvalidEmail_ReturnsBadRequest()
    {
        using var harness = await CreateHarness();
        using var client = CreateAuthenticatedClient(harness.Factory);

        var response = await client.GetAsync("/api/email/destinatario/invalido");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("e-mail", await response.Content.ReadAsStringAsync(), StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("ENVIADO")]
    [InlineData("2")]
    public async Task GetEmailsByStatus_FiltersStatus(string status)
    {
        using var harness = await CreateHarness();
        using var client = CreateAuthenticatedClient(harness.Factory);
        await SendValidEmail(client, "ok@civitas.app");

        var response = await client.GetAsync($"/api/email/status/{status}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var envelope = await response.Content.ReadFromJsonAsync<ResponseEnvelope<List<EmailData>>>(JsonOptions);
        var item = Assert.Single(envelope!.Data!);
        Assert.Equal((int)EmailStatusEnum.ENVIADO, item.Status);
    }

    [Fact]
    public async Task GetEmailsByStatus_WithInvalidStatus_ReturnsBadRequest()
    {
        using var harness = await CreateHarness();
        using var client = CreateAuthenticatedClient(harness.Factory);

        var response = await client.GetAsync("/api/email/status/INVALIDO");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("status", await response.Content.ReadAsStringAsync(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PostEmail_WithoutToken_ReturnsUnauthorized()
    {
        using var harness = await CreateHarness(devMode: false);
        using var client = harness.Factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/email", CreatePayload());

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<EmailHarness> CreateHarness(bool senderFails = false, bool? devMode = null)
    {
        var sender = new FakeEmailSender { ShouldFail = senderFails };
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            if (devMode.HasValue)
            {
                builder.ConfigureAppConfiguration((_, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["DEV"] = devMode.Value ? "true" : "false"
                    });
                });
            }

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IEmailSender>();
                services.AddSingleton<IEmailSender>(sender);
            });
        });

        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        return new EmailHarness(factory, sender);
    }

    private static async Task<EmailData> SendValidEmail(HttpClient client, string destinatario)
    {
        var response = await client.PostAsJsonAsync("/api/email", CreatePayload(destinatario));
        response.EnsureSuccessStatusCode();
        var envelope = await response.Content.ReadFromJsonAsync<ResponseEnvelope<EmailData>>(JsonOptions);
        return envelope!.Data!;
    }

    private static Dictionary<string, string> CreatePayload(
        string emailDestinatario = "usuario@civitas.app",
        string assunto = "Assunto do e-mail",
        string conteudo = "conteudo da mensagem")
    {
        return new Dictionary<string, string>
        {
            ["emailDestinatario"] = emailDestinatario,
            ["assunto"] = assunto,
            ["conteudo"] = conteudo
        };
    }

    private static string[] ReadErrors(JsonDocument document)
    {
        return document.RootElement.GetProperty("data")
            .EnumerateArray()
            .Select(error => error.GetString() ?? string.Empty)
            .ToArray();
    }

    private static HttpClient CreateAuthenticatedClient(WebApplicationFactory<FornecedorController> factory)
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", CreateTestToken());
        return client;
    }

    private static string CreateTestToken()
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("development-only-key-change-before-production-2026"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: "Civitas.WebAPI",
            audience: "Civitas.Client",
            claims: new[] { new Claim(JwtRegisteredClaimNames.Sub, "1") },
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private sealed class EmailHarness : IDisposable
    {
        public EmailHarness(WebApplicationFactory<FornecedorController> factory, FakeEmailSender sender)
        {
            Factory = factory;
            Sender = sender;
        }

        public WebApplicationFactory<FornecedorController> Factory { get; }

        public FakeEmailSender Sender { get; }

        public void Dispose()
        {
            Factory.Dispose();
        }
    }

    private sealed class FakeEmailSender : IEmailSender
    {
        public bool ShouldFail { get; set; }

        public int Calls { get; private set; }

        public (string Destinatario, string Assunto, string Conteudo)? LastMessage { get; private set; }

        public Task SendAsync(string destinatario, string assunto, string conteudo, CancellationToken cancellationToken = default)
        {
            Calls++;
            LastMessage = (destinatario, assunto, conteudo);

            if (ShouldFail)
            {
                throw new InvalidOperationException("smtp-indisponivel");
            }

            return Task.CompletedTask;
        }
    }

    private sealed record ResponseEnvelope<T>(int Code, string? Message, T? Data);

    private sealed record EmailData(
        int Id,
        string EmailDestinatario,
        string Assunto,
        string Conteudo,
        int Status,
        string? MensagemErro,
        DateTime? DataEnvio,
        DateTime DataCriacao);
}
