using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Civitas.WebAPI.Data;
using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace Civitas.WebAPI.Tests;

public sealed class TipoInstituicaoValidationEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public TipoInstituicaoValidationEndpointsTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Post_WithEmptyDescriptionAndInvalidSituacao_ReturnsAllErrorsWithoutInternalDetails()
    {
        await _factory.ResetDatabaseAsync(_ => Task.CompletedTask);

        using var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync(
            "/api/tipo-instituicao",
            CreatePayload(descricao: "   ", situacao: 0));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);
        var root = document.RootElement;
        var errors = root.GetProperty("data")
            .EnumerateArray()
            .Select(error => error.GetString() ?? string.Empty)
            .ToArray();

        Assert.Equal((int)ResponseEnum.INVALID, root.GetProperty("code").GetInt32());
        Assert.Contains(errors, error => error.Contains("Descrição é obrigatório", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(errors, error => error.Contains("Situação inválida", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain("StackTrace", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("ErrorMessage", body, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(2, "mínimo 3")]
    [InlineData(151, "máximo 150")]
    public async Task Post_WithDescriptionOutsideLimits_ReturnsBadRequest(int length, string expectedMessage)
    {
        await _factory.ResetDatabaseAsync(_ => Task.CompletedTask);

        using var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync(
            "/api/tipo-instituicao",
            CreatePayload(descricao: new string('a', length)));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(expectedMessage, await response.Content.ReadAsStringAsync(), StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(150)]
    public async Task Post_WithDescriptionAtValidLimits_PersistsValue(int length)
    {
        await _factory.ResetDatabaseAsync(_ => Task.CompletedTask);

        using var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync(
            "/api/tipo-instituicao",
            CreatePayload(descricao: new string('a', length)));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal(length, Assert.Single(context.TipoInstituicoes).Descricao.Length);
    }

    [Fact]
    public async Task Post_NormalizesDescriptionBeforePersisting()
    {
        await _factory.ResetDatabaseAsync(_ => Task.CompletedTask);

        using var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync(
            "/api/tipo-instituicao",
            CreatePayload(descricao: "   ESCOLA    MUNICIPAL   "));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal("Escola Municipal", Assert.Single(context.TipoInstituicoes).Descricao);
    }

    [Theory]
    [InlineData("escola municipal")]
    [InlineData(" ESCOLA MUNICIPAL ")]
    [InlineData("  escola    municipal  ")]
    public async Task Post_WithEquivalentDescription_ReturnsBadRequest(string descricao)
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            context.TipoInstituicoes.Add(new TipoInstituicao(1, "Escola Municipal", Situacao.ATIVO));
            return Task.CompletedTask;
        });

        using var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync(
            "/api/tipo-instituicao",
            CreatePayload(descricao: descricao));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(
            "Já existe um tipo de instituição cadastrado com esta descrição",
            await response.Content.ReadAsStringAsync(),
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Post_WithDescriptionUsedOnlyByDeletedRecord_IsAllowed()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            context.TipoInstituicoes.Add(new TipoInstituicao(1, "Escola", Situacao.ATIVO)
            {
                Excluido = true,
                DataExclusao = DateTime.UtcNow.AddDays(-1)
            });
            return Task.CompletedTask;
        });

        using var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync(
            "/api/tipo-instituicao",
            CreatePayload(descricao: " escola "));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal(2, context.TipoInstituicoes.Count());
    }

    [Fact]
    public async Task Put_WithOwnEquivalentDescription_IsAllowedAndPreservesSoftDeleteState()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            context.TipoInstituicoes.Add(new TipoInstituicao(1, "Escola Municipal", Situacao.ATIVO));
            return Task.CompletedTask;
        });

        using var client = CreateAuthenticatedClient();
        var payload = CreatePayload(descricao: " escola municipal ");
        payload["excluido"] = true;
        payload["dataExclusao"] = DateTime.UtcNow;

        var response = await client.PutAsJsonAsync("/api/tipo-instituicao/1", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var tipoInstituicao = Assert.Single(context.TipoInstituicoes);

        Assert.Equal("Escola Municipal", tipoInstituicao.Descricao);
        Assert.False(tipoInstituicao.Excluido);
        Assert.Null(tipoInstituicao.DataExclusao);
    }

    [Theory]
    [InlineData(false, 0)]
    [InlineData(false, 3)]
    [InlineData(false, -1)]
    [InlineData(true, 0)]
    [InlineData(true, 3)]
    [InlineData(true, -1)]
    public async Task PostAndPut_WithInvalidSituacao_ReturnBadRequest(bool usePut, int situacao)
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            if (usePut)
            {
                context.TipoInstituicoes.Add(new TipoInstituicao(1, "Escola", Situacao.ATIVO));
            }

            return Task.CompletedTask;
        });

        using var client = CreateAuthenticatedClient();
        var payload = CreatePayload(descricao: usePut ? "Escola" : $"Tipo {situacao}", situacao: situacao);
        var response = usePut
            ? await client.PutAsJsonAsync("/api/tipo-instituicao/1", payload)
            : await client.PostAsJsonAsync("/api/tipo-instituicao", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Situação inválida", await response.Content.ReadAsStringAsync(), StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task PutAndPatch_WithActiveLinkedInstitution_BlockInactivation(bool usePatch)
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            SeedLinkedInstitution(context, Situacao.ATIVO, Situacao.ATIVO);
            return Task.CompletedTask;
        });

        using var client = CreateAuthenticatedClient();
        var response = usePatch
            ? await client.PatchAsync("/api/tipo-instituicao/situacao/1", null)
            : await client.PutAsJsonAsync(
                "/api/tipo-instituicao/1",
                CreatePayload(descricao: "Escola", situacao: (int)Situacao.INATIVO));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);
        Assert.Equal((int)ResponseEnum.INVALID, document.RootElement.GetProperty("code").GetInt32());
        Assert.Contains("instituições ativas vinculadas", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("StackTrace", body, StringComparison.OrdinalIgnoreCase);

        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal(Situacao.ATIVO, Assert.Single(context.TipoInstituicoes).Situacao);
    }

    [Fact]
    public async Task Patch_WithoutActiveLinkedInstitution_AllowsInactivation()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            context.TipoInstituicoes.Add(new TipoInstituicao(1, "Escola", Situacao.ATIVO));
            return Task.CompletedTask;
        });

        using var client = CreateAuthenticatedClient();
        var response = await client.PatchAsync("/api/tipo-instituicao/situacao/1", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal(Situacao.INATIVO, Assert.Single(context.TipoInstituicoes).Situacao);
    }

    [Fact]
    public async Task Patch_FromInactiveToActive_IsAllowedEvenWithActiveLinkedInstitution()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            SeedLinkedInstitution(context, Situacao.INATIVO, Situacao.ATIVO);
            return Task.CompletedTask;
        });

        using var client = CreateAuthenticatedClient();
        var response = await client.PatchAsync("/api/tipo-instituicao/situacao/1", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal(Situacao.ATIVO, Assert.Single(context.TipoInstituicoes).Situacao);
    }

    [Fact]
    public async Task GetAll_ReturnsOnlyActiveTipoInstituicao()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            context.TipoInstituicoes.AddRange(
                new TipoInstituicao(1, "Escola", Situacao.ATIVO),
                new TipoInstituicao(2, "Hospital", Situacao.INATIVO));
            return Task.CompletedTask;
        });

        using var client = CreateAuthenticatedClient();
        var response = await client.GetAsync("/api/tipo-instituicao");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var items = document.RootElement.GetProperty("data").GetProperty("items");
        Assert.Single(items.EnumerateArray());
        Assert.Equal(1, items[0].GetProperty("id").GetInt32());
    }

    [Fact]
    public async Task Put_WithMissingId_ReturnsNotFound()
    {
        await _factory.ResetDatabaseAsync(_ => Task.CompletedTask);

        using var client = CreateAuthenticatedClient();
        var response = await client.PutAsJsonAsync(
            "/api/tipo-instituicao/999",
            CreatePayload(descricao: "Escola"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal((int)ResponseEnum.NOT_FOUND, document.RootElement.GetProperty("code").GetInt32());
    }

    private static Dictionary<string, object?> CreatePayload(
        string descricao = "Escola",
        int situacao = (int)Situacao.ATIVO)
    {
        return new Dictionary<string, object?>
        {
            ["descricao"] = descricao,
            ["situacao"] = situacao
        };
    }

    private static void SeedLinkedInstitution(
        AppDbContext context,
        Situacao tipoSituacao,
        Situacao instituicaoSituacao)
    {
        context.TipoInstituicoes.Add(new TipoInstituicao(1, "Escola", tipoSituacao));
        context.Secretarias.Add(new Secretaria(
            1,
            Situacao.ATIVO,
            "Secretaria de Testes",
            "04252011000110",
            "Secretaria de Testes",
            "Rua B",
            "10",
            "Centro",
            "87060000",
            "Secretaria de Testes LTDA",
            "secretaria@example.com",
            "4433334444",
            "Maringa",
            "PR"));
        context.Instituicoes.Add(new Instituicao(
            1,
            "11444777000161",
            "Instituicao Teste",
            "Rua C",
            "20",
            "Centro",
            "87060000",
            "Instituicao Teste LTDA",
            "4433335555",
            "instituicao@example.com",
            "Maringa",
            "PR",
            instituicaoSituacao)
        {
            IdSecretaria = 1,
            IdTipoInstituicao = 1
        });
    }

    private HttpClient CreateAuthenticatedClient()
    {
        var client = _factory.CreateClient();
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
}
