using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Civitas.WebAPI.Data;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace Civitas.WebAPI.Tests;

public sealed class TipoDespesaValidationEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public TipoDespesaValidationEndpointsTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostTipoDespesa_WithNormalizedDescription_PersistsStandardizedValue()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            context.UnidadesMedida.Add(new UnidadeMedida(1, "Quilowatt-hora", "kWh", Situacao.ATIVO));
            context.TipoCodigos.Add(new TipoCodigo(1, "Conta", "Conta de consumo"));
            return Task.CompletedTask;
        });

        using var client = CreateAuthenticatedClient();
        var request = CreateTipoDespesaPayload(descricao: "  energia   elétrica  ");

        var response = await client.PostAsJsonAsync("/api/tipo-despesa", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var tipoDespesa = Assert.Single(context.TiposDespesa);

        Assert.Equal("Energia Elétrica", tipoDespesa.Descricao);
    }

    [Fact]
    public async Task PostTipoDespesa_WithMultipleInvalidFields_ReturnsAllValidationErrors()
    {
        await _factory.ResetDatabaseAsync(_ => Task.CompletedTask);

        using var client = CreateAuthenticatedClient();
        var request = CreateTipoDespesaPayload(
            descricao: " a ",
            solicitaUc: 99,
            situacao: 99,
            idUnidadeMedida: 0,
            idTipoCodigo: 0);

        var response = await client.PostAsJsonAsync("/api/tipo-despesa", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("no mínimo 3", body, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("IdUnidadeMedida", body, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("IdTipoCodigo", body, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("SolicitaUc", body, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Situação", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PostTipoDespesa_WithDuplicateDescriptionIgnoringCase_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            context.UnidadesMedida.Add(new UnidadeMedida(1, "Quilowatt-hora", "kWh", Situacao.ATIVO));
            context.TipoCodigos.Add(new TipoCodigo(1, "Conta", "Conta de consumo"));
            context.TiposDespesa.Add(new TipoDespesa(1, "Energia", SolicitaUc.Sim, Situacao.ATIVO)
            {
                IdUnidadeMedida = 1,
                IdTipoCodigo = 1
            });
            return Task.CompletedTask;
        });

        using var client = CreateAuthenticatedClient();
        var request = CreateTipoDespesaPayload(descricao: " energia ");

        var response = await client.PostAsJsonAsync("/api/tipo-despesa", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Já existe um tipo de despesa cadastrado com esta descrição", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PatchTipoDespesa_WithDespesaVinculada_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            context.UnidadesMedida.Add(new UnidadeMedida(1, "Quilowatt-hora", "kWh", Situacao.ATIVO));
            context.TipoCodigos.Add(new TipoCodigo(1, "Conta", "Conta de consumo"));
            context.TiposDespesa.Add(new TipoDespesa(1, "Energia", SolicitaUc.Sim, Situacao.ATIVO)
            {
                IdUnidadeMedida = 1,
                IdTipoCodigo = 1
            });

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

            context.TipoInstituicoes.Add(new TipoInstituicao(1, "Escola", Situacao.ATIVO));

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
                Situacao.ATIVO)
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
                "Rua D",
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
                "Fornecedor Ativo",
                Situacao.ATIVO,
                "11222333000181",
                "Fornecedor Ativo LTDA",
                "Rua E",
                "40",
                "Centro",
                "87060000",
                "44999999999",
                "ativo@fornecedor.com",
                "Maringa",
                "PR"));

            context.Despesas.Add(new Despesa(1, "123456", "123", "2026-03-10", 12.5, new DateOnly(2026, 3, 20), Status.A_PAGAR)
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

        var response = await client.PatchAsync("/api/tipo-despesa/situacao/1", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("despesas vinculadas", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PostDespesa_WithInactiveTipoDespesa_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            context.UnidadesMedida.Add(new UnidadeMedida(1, "Quilowatt-hora", "kWh", Situacao.ATIVO));
            context.TipoCodigos.Add(new TipoCodigo(1, "Conta", "Conta de consumo"));
            context.TiposDespesa.Add(new TipoDespesa(1, "Energia", SolicitaUc.Não, Situacao.INATIVO)
            {
                IdUnidadeMedida = 1,
                IdTipoCodigo = 1
            });

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

            context.TipoInstituicoes.Add(new TipoInstituicao(1, "Escola", Situacao.ATIVO));

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
                Situacao.ATIVO)
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
                "Rua D",
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
                "Fornecedor Ativo",
                Situacao.ATIVO,
                "11222333000181",
                "Fornecedor Ativo LTDA",
                "Rua E",
                "40",
                "Centro",
                "87060000",
                "44999999999",
                "ativo@fornecedor.com",
                "Maringa",
                "PR"));

            return Task.CompletedTask;
        });

        using var client = CreateAuthenticatedClient();
        var request = new Dictionary<string, object?>
        {
            ["numeroDocumento"] = "123456",
            ["uc"] = string.Empty,
            ["dataEmissao"] = "2026-03-10",
            ["consumoPrevisto"] = 12.5,
            ["dataVencimento"] = "2026-03-20",
            ["idTipoDespesa"] = 1,
            ["idOrcamento"] = 1,
            ["idInstituicao"] = 1,
            ["idFornecedor"] = 1,
            ["idUsuario"] = 1
        };

        var response = await client.PostAsJsonAsync("/api/despesas", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("inativo", body, StringComparison.OrdinalIgnoreCase);
    }

    private static Dictionary<string, object?> CreateTipoDespesaPayload(
        string descricao = "Energia",
        int solicitaUc = (int)SolicitaUc.Sim,
        int situacao = (int)Situacao.ATIVO,
        int idUnidadeMedida = 1,
        int idTipoCodigo = 1)
    {
        return new Dictionary<string, object?>
        {
            ["descricao"] = descricao,
            ["solicitaUc"] = solicitaUc,
            ["situacao"] = situacao,
            ["idUnidadeMedida"] = idUnidadeMedida,
            ["idTipoCodigo"] = idTipoCodigo
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
