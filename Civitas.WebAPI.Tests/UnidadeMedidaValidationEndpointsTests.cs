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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace Civitas.WebAPI.Tests;

public sealed class UnidadeMedidaValidationEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public UnidadeMedidaValidationEndpointsTests(TestWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task PostUnidadeMedida_WithMultipleInvalidFields_ReturnsAllValidationErrors()
    {
        await _factory.ResetDatabaseAsync(_ => Task.CompletedTask);
        using var client = CreateAuthenticatedClient();

        var response = await client.PostAsJsonAsync("/api/unidade-medida", new
        {
            descricao = " ",
            abreviatura = " ",
            situacao = 99
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Descrição", body);
        Assert.Contains("Abreviatura", body);
        Assert.Contains("Situação", body);
        Assert.DoesNotContain("StackTrace", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PostUnidadeMedida_NormalizesValuesAndRejectsCaseInsensitiveDuplicates()
    {
        await _factory.ResetDatabaseAsync(_ => Task.CompletedTask);
        using var client = CreateAuthenticatedClient();

        var created = await client.PostAsJsonAsync("/api/unidade-medida", Payload("  metro   cúbico  ", " m3 "));
        Assert.Equal(HttpStatusCode.OK, created.StatusCode);

        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var unidade = Assert.Single(context.UnidadesMedida);
            Assert.Equal("Metro Cúbico", unidade.Descricao);
            Assert.Equal("M3", unidade.Abreviatura);
        }

        var duplicate = await client.PostAsJsonAsync("/api/unidade-medida", Payload("METRO CÚBICO", "m3"));
        Assert.Equal(HttpStatusCode.BadRequest, duplicate.StatusCode);
        var body = await duplicate.Content.ReadAsStringAsync();
        Assert.Contains("descrição", body, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("abreviatura", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PutUnidadeMedida_InactivatingWithActiveTipoDespesa_ReturnsBadRequest()
    {
        await SeedUnidadeWithActiveTipoDespesaAsync();
        using var client = CreateAuthenticatedClient();

        var response = await client.PutAsJsonAsync("/api/unidade-medida/1", Payload("Quilowatt-hora", "kwh", Situacao.INATIVO));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("tipos de despesa ativos", await response.Content.ReadAsStringAsync(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PatchSituacao_InactivatingWithActiveTipoDespesa_ReturnsBadRequest()
    {
        await SeedUnidadeWithActiveTipoDespesaAsync();
        using var client = CreateAuthenticatedClient();

        var response = await client.PatchAsync("/api/unidade-medida/situacao/1", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("tipos de despesa ativos", await response.Content.ReadAsStringAsync(), StringComparison.OrdinalIgnoreCase);
    }

    private async Task SeedUnidadeWithActiveTipoDespesaAsync()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            context.UnidadesMedida.Add(new UnidadeMedida(1, "Quilowatt-hora", "KWH", Situacao.ATIVO));
            context.TipoCodigos.Add(new TipoCodigo(1, "Consumo", "Código de consumo"));
            context.TiposDespesa.Add(new TipoDespesa(1, "Energia", SolicitaUc.Sim, Situacao.ATIVO)
            {
                IdUnidadeMedida = 1,
                IdTipoCodigo = 1
            });
            return Task.CompletedTask;
        });
    }

    private static object Payload(string descricao, string abreviatura, Situacao situacao = Situacao.ATIVO) => new
    {
        descricao,
        abreviatura,
        situacao = (int)situacao
    };

    private System.Net.Http.HttpClient CreateAuthenticatedClient()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateTestToken());
        return client;
    }

    private static string CreateTestToken()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("development-only-key-change-before-production-2026"));
        var token = new JwtSecurityToken(
            issuer: "Civitas.WebAPI",
            audience: "Civitas.Client",
            claims: [new Claim(JwtRegisteredClaimNames.Sub, "1")],
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
