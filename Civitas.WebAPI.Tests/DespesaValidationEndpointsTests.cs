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

public sealed class DespesaValidationEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public DespesaValidationEndpointsTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostDespesa_WithMultipleInvalidFields_ReturnsAllValidationErrors()
    {
        await _factory.ResetDatabaseAsync(_ => Task.CompletedTask);

        using var client = CreateAuthenticatedClient();
        var request = CreateDespesaPayload(
            numeroDocumento: "",
            codigo: "",
            dataEmissao: "2026-03-20",
            dataVencimento: "2026-03-10",
            valorPrevisto: -1m,
            valorPago: -1m,
            consumoPrevisto: -1m,
            consumoReal: -1m,
            status: 99,
            idUsuario: 0,
            idUnidadeConsumidora: 0);

        var response = await client.PostAsJsonAsync("/api/despesas", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("NumeroDocumento", body, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Codigo", body, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("DataVencimento", body, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ValorPrevisto", body, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ValorPago", body, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ConsumoPrevisto", body, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ConsumoReal", body, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("IdUsuario", body, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("IdUnidadeConsumidora", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PostDespesa_WithInvalidUsuario_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            SeedDespesaDependencies(context);
            return Task.CompletedTask;
        });

        using var client = CreateAuthenticatedClient();
        var request = CreateDespesaPayload(idUsuario: 999);

        var response = await client.PostAsJsonAsync("/api/despesas", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Usuario", body, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("nao foi encontrado", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PostDespesa_WithInactiveUnidadeConsumidora_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            SeedDespesaDependencies(context, unidadeConsumidoraSituacao: Situacao.INATIVO);
            return Task.CompletedTask;
        });

        using var client = CreateAuthenticatedClient();
        var request = CreateDespesaPayload();

        var response = await client.PostAsJsonAsync("/api/despesas", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("UnidadeConsumidora", body, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("inativa", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PostDespesa_AlwaysStartsWithStatusAPagar()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            SeedDespesaDependencies(context);
            return Task.CompletedTask;
        });

        using var client = CreateAuthenticatedClient();
        var request = CreateDespesaPayload(status: (int)Status.PAGA);

        var response = await client.PostAsJsonAsync("/api/despesas", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var despesa = Assert.Single(context.Despesas);
        Assert.Equal(Status.A_PAGAR, despesa.Status);
    }

    [Fact]
    public async Task PatchDespesaStatus_WithInvalidStatus_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            SeedDespesaDependencies(context);
            context.Despesas.Add(CreateDespesa(1, "DOC-001", "DESP-001", idUnidadeConsumidora: 1));

            return Task.CompletedTask;
        });

        using var client = CreateAuthenticatedClient();
        var response = await client.PatchAsJsonAsync("/api/despesas/1/status", 99);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Status", body, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("invalido", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetDespesasByUnidadeConsumidora_ReturnsOnlyMatchingDespesas()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            SeedDespesaDependencies(context);
            context.UnidadesConsumidoras.Add(new UnidadeConsumidora(2, "UC-002", Situacao.ATIVO, 1, 1, 1, 1));
            context.Despesas.Add(CreateDespesa(1, "DOC-001", "DESP-001", idUnidadeConsumidora: 1));
            context.Despesas.Add(CreateDespesa(2, "DOC-002", "DESP-002", idUnidadeConsumidora: 2));

            return Task.CompletedTask;
        });

        using var client = CreateAuthenticatedClient();
        var response = await client.GetAsync("/api/despesas/unidade-consumidora/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("DOC-001", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("DOC-002", body, StringComparison.OrdinalIgnoreCase);
    }

    private static Dictionary<string, object?> CreateDespesaPayload(
        string numeroDocumento = "DOC-001",
        string codigo = "DESP-001",
        string dataEmissao = "2026-03-10",
        decimal valorPrevisto = 100m,
        decimal valorPago = 0m,
        decimal consumoPrevisto = 50m,
        decimal consumoReal = 0m,
        string dataVencimento = "2026-03-20",
        int status = (int)Status.A_PAGAR,
        int idUsuario = 1,
        int idUnidadeConsumidora = 1)
    {
        return new Dictionary<string, object?>
        {
            ["numeroDocumento"] = numeroDocumento,
            ["codigo"] = codigo,
            ["dataEmissao"] = dataEmissao,
            ["valorPrevisto"] = valorPrevisto,
            ["valorPago"] = valorPago,
            ["consumoPrevisto"] = consumoPrevisto,
            ["consumoReal"] = consumoReal,
            ["dataVencimento"] = dataVencimento,
            ["status"] = status,
            ["idUsuario"] = idUsuario,
            ["idUnidadeConsumidora"] = idUnidadeConsumidora
        };
    }

    private static void SeedDespesaDependencies(
        AppDbContext context,
        Situacao unidadeConsumidoraSituacao = Situacao.ATIVO,
        Situacao usuarioSituacao = Situacao.ATIVO)
    {
        context.UnidadesMedida.Add(new UnidadeMedida(1, "Quilowatt-hora", "kWh", Situacao.ATIVO));
        context.TipoCodigos.Add(new TipoCodigo(1, "Consumo", "Tipo de codigo de teste"));

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

        context.Orcamentos.Add(new Orcamento(1, 2026, 5000m, 1));

        context.Fornecedores.Add(new Fornecedor(
            1,
            "Fornecedor Teste",
            Situacao.ATIVO,
            "11222333000181",
            "Fornecedor Teste LTDA",
            "Rua E",
            "40",
            "Centro",
            "87060000",
            "44999999999",
            "fornecedor@example.com",
            "Maringa",
            "PR"));

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
            usuarioSituacao,
            "MAT-001",
            TipoUsuario.ADMINISTRADOR,
            "Centro"));

        context.UnidadesConsumidoras.Add(new UnidadeConsumidora(
            1,
            "UC-001",
            unidadeConsumidoraSituacao,
            1,
            1,
            1,
            1));
    }

    private static Despesa CreateDespesa(
        int id,
        string numeroDocumento,
        string codigo,
        int idUnidadeConsumidora)
    {
        return new Despesa(
            id,
            numeroDocumento,
            codigo,
            new DateOnly(2026, 3, 10),
            100m,
            0m,
            50m,
            0m,
            new DateOnly(2026, 3, 20),
            Status.A_PAGAR,
            1,
            idUnidadeConsumidora);
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
