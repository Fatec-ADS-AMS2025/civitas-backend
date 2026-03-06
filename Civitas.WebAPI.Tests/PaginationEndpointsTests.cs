using System.Net;
using System.Text.Json;
using Civitas.WebAPI.Data;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Tests.Infrastructure;
using Xunit;

namespace Civitas.WebAPI.Tests;

public sealed class PaginationEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly TestWebApplicationFactory _factory;

    public PaginationEndpointsTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetFornecedores_ReturnsPaginatedPayloadWithOrderingMetadata()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            context.Fornecedores.AddRange(CreateFornecedores(12));
            return Task.CompletedTask;
        });

        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/fornecedores?page=2&size=5&sortBy=NomeFantasia&sortDirection=desc");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var data = document.RootElement.GetProperty("data");
        var items = data.GetProperty("items");

        Assert.Equal(12, data.GetProperty("totalRecords").GetInt32());
        Assert.Equal(3, data.GetProperty("totalPages").GetInt32());
        Assert.Equal(2, data.GetProperty("currentPage").GetInt32());
        Assert.Equal(5, items.GetArrayLength());
        Assert.Equal("Fornecedor 07", items[0].GetProperty("nomeFantasia").GetString());
    }

    [Fact]
    public async Task GetSecretarias_ClampsSizeAndFallsBackToPrimaryKeyOrdering()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            context.Secretarias.AddRange(CreateSecretarias(110));
            return Task.CompletedTask;
        });

        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/secretarias?page=0&size=500&sortBy=Inexistente&sortDirection=desc");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var data = document.RootElement.GetProperty("data");
        var items = data.GetProperty("items");

        Assert.Equal(110, data.GetProperty("totalRecords").GetInt32());
        Assert.Equal(2, data.GetProperty("totalPages").GetInt32());
        Assert.Equal(1, data.GetProperty("currentPage").GetInt32());
        Assert.Equal(100, data.GetProperty("pageSize").GetInt32());
        Assert.Equal(100, items.GetArrayLength());
        Assert.Equal(110, items[0].GetProperty("idSecretaria").GetInt32());
    }

    private static IEnumerable<Fornecedor> CreateFornecedores(int total)
    {
        for (var index = 1; index <= total; index++)
        {
            yield return new Fornecedor(
                index,
                $"Fornecedor {index:D2}",
                Situacao.ATIVO,
                $"{index:D14}",
                $"Fornecedor Razao {index:D2}",
                "Rua A",
                index.ToString(),
                "Centro",
                "00000000",
                "44999999999",
                $"fornecedor{index:D2}@example.com",
                "Cidade",
                "PR");
        }
    }

    private static IEnumerable<Secretaria> CreateSecretarias(int total)
    {
        for (var index = 1; index <= total; index++)
        {
            yield return new Secretaria(
                index,
                Situacao.ATIVO,
                $"Descricao {index:D3}",
                $"{index:D14}",
                $"Secretaria {index:D3}",
                "Rua B",
                index.ToString(),
                "Centro",
                "00000000",
                $"Secretaria Razao {index:D3}",
                $"secretaria{index:D3}@example.com",
                "44999999999",
                "Cidade",
                "PR");
        }
    }
}
