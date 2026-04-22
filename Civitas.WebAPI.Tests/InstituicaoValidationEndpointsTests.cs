using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Civitas.WebAPI.Data;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace Civitas.WebAPI.Tests;

public sealed class InstituicaoValidationEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public InstituicaoValidationEndpointsTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostInstituicao_WithMaskedFields_PersistsNormalizedValues()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            SeedInstituicaoDependencies(context);
            return Task.CompletedTask;
        });

        using var client = CreateAuthenticatedClient();
        var request = CreateInstituicaoPayload(
            nome: "  Escola Aurora  ",
            nomeRazaoSocial: "  Escola Aurora LTDA  ",
            cnpj: "11.222.333/0001-81",
            cep: "87.060-000",
            telefone: "(44) 3333-4444",
            email: "  CONTATO@ESCOLA.COM  ",
            estado: "pr");

        var response = await client.PostAsJsonAsync("/api/instituicoes", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var instituicao = Assert.Single(context.Instituicoes);

        Assert.Equal("Escola Aurora", instituicao.Nome);
        Assert.Equal("Escola Aurora LTDA", instituicao.NomeRazaoSocial);
        Assert.Equal("11222333000181", instituicao.CNPJ);
        Assert.Equal("87060000", instituicao.CEP);
        Assert.Equal("4433334444", instituicao.Telefone);
        Assert.Equal("contato@escola.com", instituicao.Email);
        Assert.Equal("PR", instituicao.Estado);
    }

    [Fact]
    public async Task PostInstituicao_WithInvalidCnpj_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            SeedInstituicaoDependencies(context);
            return Task.CompletedTask;
        });

        using var client = CreateAuthenticatedClient();
        var request = CreateInstituicaoPayload(cnpj: "11.111.111/1111-11");

        var response = await client.PostAsJsonAsync("/api/instituicoes", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("CNPJ", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PostInstituicao_WithDuplicateCnpj_ReturnsConflict()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            SeedInstituicaoDependencies(context);
            context.Instituicoes.Add(CreateInstituicao(
                id: 1,
                cnpj: "11222333000181",
                email: "existente@instituicao.com"));
            return Task.CompletedTask;
        });

        using var client = CreateAuthenticatedClient();
        var request = CreateInstituicaoPayload(
            cnpj: "11.222.333/0001-81",
            email: "nova@instituicao.com");

        var response = await client.PostAsJsonAsync("/api/instituicoes", request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("cnpj", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PostInstituicao_WithDuplicateEmail_ReturnsConflict()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            SeedInstituicaoDependencies(context);
            context.Instituicoes.Add(CreateInstituicao(
                id: 1,
                cnpj: "11444777000161",
                email: "duplicado@instituicao.com"));
            return Task.CompletedTask;
        });

        using var client = CreateAuthenticatedClient();
        var request = CreateInstituicaoPayload(
            cnpj: "11.222.333/0001-81",
            email: "DUPLICADO@INSTITUICAO.COM");

        var response = await client.PostAsJsonAsync("/api/instituicoes", request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("email", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PostInstituicao_WithInvalidForeignKeys_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            SeedInstituicaoDependencies(context);
            return Task.CompletedTask;
        });

        using var client = CreateAuthenticatedClient();
        var request = CreateInstituicaoPayload();
        request["idTipoInstituicao"] = 999;

        var response = await client.PostAsJsonAsync("/api/instituicoes", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostInstituicao_WithMultipleErrors_ReturnsAllErrorsAtOnce()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            SeedInstituicaoDependencies(context);
            return Task.CompletedTask;
        });

        using var client = CreateAuthenticatedClient();
        var request = CreateInstituicaoPayload(
            cnpj: "",
            nome: " ",
            email: "email-invalido");

        var response = await client.PostAsJsonAsync("/api/instituicoes", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var data = document.RootElement.GetProperty("data");
        Assert.Equal(JsonValueKind.Array, data.ValueKind);
        Assert.True(data.GetArrayLength() >= 3);
    }

    [Fact]
    public async Task PutInstituicao_WithValidData_UpdatesSuccessfully()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            SeedInstituicaoDependencies(context);
            context.Instituicoes.Add(CreateInstituicao(
                id: 1,
                cnpj: "11444777000161",
                email: "antigo@instituicao.com"));
            return Task.CompletedTask;
        });

        using var client = CreateAuthenticatedClient();
        var request = CreateInstituicaoPayload(
            nome: "Escola Atualizada",
            nomeRazaoSocial: "Escola Atualizada LTDA",
            cnpj: "11.222.333/0001-81",
            email: "ATUALIZADA@INSTITUICAO.COM",
            telefone: "(44) 99999-1111");

        var response = await client.PutAsJsonAsync("/api/instituicoes/1", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var instituicao = await context.Instituicoes.FindAsync(1);

        Assert.NotNull(instituicao);
        Assert.Equal("Escola Atualizada", instituicao!.Nome);
        Assert.Equal("11222333000181", instituicao.CNPJ);
        Assert.Equal("atualizada@instituicao.com", instituicao.Email);
        Assert.Equal("44999991111", instituicao.Telefone);
    }

    [Fact]
    public async Task PatchInstituicao_ToInactiveWithPendingExpenses_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            SeedDespesaDependencies(context, Situacao.ATIVO);
            context.Despesas.Add(new Despesa(
                1,
                "123456",
                string.Empty,
                "10-03-2026",
                12.5,
                new DateOnly(2026, 3, 20),
                Status.A_PAGAR)
            {
                IdTipoDespesa = 1,
                IdOrcamento = 1,
                IdInstituicao = 1,
                IdFornecedor = 1,
                IdUsuario = 1
            });

            return Task.CompletedTask;
        });

        using var client = CreateAuthenticatedClient();
        var response = await client.PatchAsync("/api/instituicoes/situacao/1", content: null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("pendente", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PostDespesa_WithInactiveInstituicao_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            SeedDespesaDependencies(context, Situacao.INATIVO);
            return Task.CompletedTask;
        });

        using var client = CreateAuthenticatedClient();
        var request = new Dictionary<string, object?>
        {
            ["numeroDocumento"] = "123456",
            ["uc"] = "",
            ["dataEmissao"] = "2026-03-10",
            ["consumoPrevisto"] = 12.5,
            ["dataVencimento"] = "2026-03-20",
            ["situacao"] = (int)Situacao.ATIVO,
            ["idTipoDespesa"] = 1,
            ["idOrcamento"] = 1,
            ["idInstituicao"] = 1,
            ["idFornecedor"] = 1,
            ["idUsuario"] = 1
        };

        var response = await client.PostAsJsonAsync("/api/despesas", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("inativa", body, StringComparison.OrdinalIgnoreCase);
    }

    private static Dictionary<string, object?> CreateInstituicaoPayload(
        string cnpj = "11.444.777/0001-61",
        string nome = "Instituicao de Teste",
        string logradouro = "Rua das Flores",
        string numero = "123",
        string bairro = "Centro",
        string cep = "87060-000",
        string nomeRazaoSocial = "Instituicao de Teste LTDA",
        string telefone = "(44) 3333-4444",
        string email = "instituicao@example.com",
        string cidade = "Maringa",
        string estado = "PR",
        int situacao = (int)Situacao.ATIVO,
        int idTipoInstituicao = 1,
        int idSecretaria = 1)
    {
        return new Dictionary<string, object?>
        {
            ["cnpj"] = cnpj,
            ["nome"] = nome,
            ["logradouro"] = logradouro,
            ["numero"] = numero,
            ["bairro"] = bairro,
            ["cep"] = cep,
            ["nomeRazaoSocial"] = nomeRazaoSocial,
            ["telefone"] = telefone,
            ["email"] = email,
            ["cidade"] = cidade,
            ["estado"] = estado,
            ["situacao"] = situacao,
            ["idTipoInstituicao"] = idTipoInstituicao,
            ["idSecretaria"] = idSecretaria
        };
    }

    private static void SeedInstituicaoDependencies(AppDbContext context)
    {
        context.Secretarias.Add(new Secretaria(
            1,
            Situacao.ATIVO,
            "Secretaria Teste",
            "04252011000110",
            "Secretaria Teste",
            "Rua A",
            "10",
            "Centro",
            "87060000",
            "Secretaria Teste LTDA",
            "secretaria@test.com",
            "4433334444",
            "Maringa",
            "PR"));

        context.TipoInstituicoes.Add(new TipoInstituicao(1, "Escola", Situacao.ATIVO));
    }

    private static void SeedDespesaDependencies(AppDbContext context, Situacao instituicaoSituacao)
    {
        SeedInstituicaoDependencies(context);

        context.UnidadesMedida.Add(new UnidadeMedida(1, "Quilowatt-hora", "kWh", Situacao.ATIVO));
        context.TipoCodigos.Add(new TipoCodigo(1, "Consumo", "Tipo de codigo de teste"));

        context.TiposDespesa.Add(new TipoDespesa(1, "Energia", SolicitaUc.Não, Situacao.ATIVO)
        {
            IdUnidadeMedida = 1,
            IdTipoCodigo = 1
        });

        context.Instituicoes.Add(new Instituicao(
            1,
            "11444777000161",
            "Instituicao Teste",
            "Rua B",
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

        context.Orcamentos.Add(new Orcamento(1, 2026, 5000, 1)
        {
            IdTipoDespesa = 1
        });

        context.Usuarios.Add(new Usuario(
            1,
            "52998224725",
            "Usuario Teste",
            "123456789",
            "Rua C",
            "30",
            "Maringa",
            "PR",
            "87060000",
            "usuario@example.com",
            "hash",
            Situacao.ATIVO,
            "MAT-001",
            TipoUsuario.ADMINISTRADOR,
            "Centro"));

        context.Fornecedores.Add(new Fornecedor(
            1,
            "Fornecedor Teste",
            Situacao.ATIVO,
            "11222333000181",
            "Fornecedor Teste LTDA",
            "Rua D",
            "40",
            "Centro",
            "87060000",
            "44999999999",
            "fornecedor@example.com",
            "Maringa",
            "PR"));
    }

    private static Instituicao CreateInstituicao(int id, string cnpj, string email)
    {
        return new Instituicao(
            id,
            cnpj,
            "Instituicao Existente",
            "Rua Seed",
            "100",
            "Centro",
            "87060000",
            "Instituicao Existente LTDA",
            "4433334444",
            email,
            "Maringa",
            "PR",
            Situacao.ATIVO)
        {
            IdTipoInstituicao = 1,
            IdSecretaria = 1
        };
    }

    private System.Net.Http.HttpClient CreateAuthenticatedClient()
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
