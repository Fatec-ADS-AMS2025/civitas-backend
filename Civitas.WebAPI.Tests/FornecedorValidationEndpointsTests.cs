using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.IdentityModel.Tokens.Jwt;
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

public sealed class FornecedorValidationEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public FornecedorValidationEndpointsTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostFornecedor_WithMaskedFields_PersistsNormalizedValues()
    {
        await _factory.ResetDatabaseAsync(_ => Task.CompletedTask);

        using var client = CreateAuthenticatedClient();
        var request = CreateFornecedorPayload(
            nomeFantasia: "  Fornecedor XPTO  ",
            nome: "  Fornecedor XPTO LTDA  ",
            cnpj: "11.222.333/0001-81",
            cep: "87.060-000",
            telefone: "(44) 99999-9999",
            email: "  CONTATO@Fornecedor.COM  ",
            estado: "pr");

        var response = await client.PostAsJsonAsync("/api/fornecedores", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var fornecedor = Assert.Single(context.Fornecedores);

        Assert.Equal("Fornecedor XPTO", fornecedor.NomeFantasia);
        Assert.Equal("Fornecedor XPTO LTDA", fornecedor.Nome);
        Assert.Equal("11222333000181", fornecedor.Cnpj);
        Assert.Equal("87060000", fornecedor.Cep);
        Assert.Equal("44999999999", fornecedor.Telefone);
        Assert.Equal("contato@fornecedor.com", fornecedor.Email);
        Assert.Equal("PR", fornecedor.Estado);
    }

    [Fact]
    public async Task PostFornecedor_WithInvalidCnpj_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync(_ => Task.CompletedTask);

        using var client = CreateAuthenticatedClient();
        var request = CreateFornecedorPayload(cnpj: "11.111.111/1111-11");

        var response = await client.PostAsJsonAsync("/api/fornecedores", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("CNPJ", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PostFornecedor_WithDuplicateCnpj_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            context.Fornecedores.Add(new Fornecedor(
                1,
                "Fornecedor Existente",
                Situacao.ATIVO,
                "11222333000181",
                "Fornecedor Existente LTDA",
                "Rua A",
                "100",
                "Centro",
                "87060000",
                "44999999999",
                "existente@fornecedor.com",
                "Maringa",
                "PR"));

            return Task.CompletedTask;
        });

        using var client = CreateAuthenticatedClient();
        var request = CreateFornecedorPayload(cnpj: "11.222.333/0001-81");

        var response = await client.PostAsJsonAsync("/api/fornecedores", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("CNPJ", body, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("cadastrado", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PostDespesa_WithInactiveFornecedor_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            context.UnidadesMedida.Add(new UnidadeMedida(1, "Quilowatt-hora", "kWh", Situacao.ATIVO));
            context.TipoCodigos.Add(new TipoCodigo(1, "Consumo", "Tipo de codigo de teste"));

            context.TiposDespesa.Add(new TipoDespesa(1, "Energia", SolicitaUc.Não, Situacao.ATIVO)
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
                "Fornecedor Inativo",
                Situacao.INATIVO,
                "11222333000181",
                "Fornecedor Inativo LTDA",
                "Rua E",
                "40",
                "Centro",
                "87060000",
                "44999999999",
                "inativo@fornecedor.com",
                "Maringa",
                "PR"));

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
        Assert.Contains("inativo", body, StringComparison.OrdinalIgnoreCase);
    }

    private static Dictionary<string, object?> CreateFornecedorPayload(
        string nomeFantasia = "Fornecedor XPTO",
        string nome = "Fornecedor XPTO LTDA",
        string cnpj = "11.222.333/0001-81",
        string logradouro = "Rua das Flores",
        string numero = "123",
        string bairro = "Centro",
        string cep = "87060-000",
        string telefone = "(44) 99999-9999",
        string email = "contato@fornecedor.com",
        string cidade = "Maringa",
        string estado = "PR",
        int situacao = (int)Situacao.ATIVO)
    {
        return new Dictionary<string, object?>
        {
            ["nomeFantasia"] = nomeFantasia,
            ["situacao"] = situacao,
            ["cnpj"] = cnpj,
            ["nome"] = nome,
            ["logradouro"] = logradouro,
            ["numero"] = numero,
            ["bairro"] = bairro,
            ["cep"] = cep,
            ["telefone"] = telefone,
            ["email"] = email,
            ["cidade"] = cidade,
            ["estado"] = estado
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
