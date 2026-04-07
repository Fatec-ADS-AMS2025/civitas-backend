using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Civitas.WebAPI.Controllers;
using Civitas.WebAPI.Services.Entities;
using Civitas.WebAPI.Services.Interfaces;
using Civitas.WebAPI.Tests.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace Civitas.WebAPI.Tests;

public sealed class CepEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly TestWebApplicationFactory _factory;

    public CepEndpointsTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetCep_WithValidCep_ReturnsEndereco()
    {
        using var factory = CreateFactory((request, _) =>
        {
            Assert.Equal("/ws/01001000/json/", request.RequestUri?.AbsolutePath);

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new
                {
                    cep = "01001-000",
                    logradouro = "Praça da Sé",
                    bairro = "Sé",
                    localidade = "São Paulo",
                    uf = "SP"
                })
            });
        });

        using var client = CreateAuthenticatedClient(factory);
        var response = await client.GetAsync("/api/cep/01001000");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ResponseEnvelope<CepData>>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal(1, body!.Code);
        Assert.Equal("Endereço encontrado com sucesso", body.Message);
        Assert.NotNull(body.Data);
        Assert.Equal("Praça da Sé", body.Data!.Logradouro);
        Assert.Equal("Sé", body.Data.Bairro);
        Assert.Equal("São Paulo", body.Data.Cidade);
        Assert.Equal("SP", body.Data.Estado);
    }

    [Fact]
    public async Task GetCep_WithMaskedCep_SanitizesBeforeCallingViaCep()
    {
        using var factory = CreateFactory((request, _) =>
        {
            Assert.Equal("/ws/01001000/json/", request.RequestUri?.AbsolutePath);

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new
                {
                    cep = "01001-000",
                    logradouro = "Praça da Sé",
                    bairro = "Sé",
                    localidade = "São Paulo",
                    uf = "SP"
                })
            });
        });

        using var client = CreateAuthenticatedClient(factory);
        var response = await client.GetAsync("/api/cep/01001-000");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/cep/123")]
    [InlineData("/api/cep/abcdefgh")]
    public async Task GetCep_WithInvalidCep_ReturnsBadRequest(string route)
    {
        using var factory = CreateFactory((_, _) =>
            throw new InvalidOperationException("O serviço externo não deveria ser chamado para CEP inválido."));

        using var client = CreateAuthenticatedClient(factory);
        var response = await client.GetAsync(route);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ResponseEnvelope<object>>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal(2, body!.Code);
        Assert.Equal("CEP deve conter exatamente 8 dígitos numéricos.", body.Message);
    }

    [Fact]
    public async Task GetCep_WhenViaCepReturnsErroTrue_ReturnsNotFound()
    {
        using var factory = CreateFactory((_, _) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new
                {
                    erro = true
                })
            }));

        using var client = CreateAuthenticatedClient(factory);
        var response = await client.GetAsync("/api/cep/99999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ResponseEnvelope<object>>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal(3, body!.Code);
        Assert.Equal("CEP não encontrado", body.Message);
    }

    [Fact]
    public async Task GetCep_WhenViaCepFails_ReturnsInternalServerError()
    {
        using var factory = CreateFactory((_, _) => throw new TaskCanceledException("timeout"));

        using var client = CreateAuthenticatedClient(factory);
        var response = await client.GetAsync("/api/cep/01001000");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ResponseEnvelope<object>>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal(6, body!.Code);
        Assert.Equal("Erro ao consultar CEP", body.Message);
    }

    [Fact]
    public async Task GetCep_WithoutToken_ReturnsUnauthorized()
    {
        using var factory = CreateFactory((_, _) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new
                {
                    cep = "01001-000",
                    logradouro = "Praça da Sé",
                    bairro = "Sé",
                    localidade = "São Paulo",
                    uf = "SP"
                })
            }));

        using var client = factory.CreateClient();
        var response = await client.GetAsync("/api/cep/01001000");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private WebApplicationFactory<FornecedorController> CreateFactory(
        Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handler)
    {
        return _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<ICepService>();
                services.AddSingleton<ICepService>(_ =>
                {
                    var client = new HttpClient(new DelegateHttpMessageHandler(handler))
                    {
                        BaseAddress = new Uri("https://viacep.com.br/"),
                        Timeout = TimeSpan.FromSeconds(10)
                    };

                    return new CepService(client);
                });
            });
        });
    }

    private System.Net.Http.HttpClient CreateAuthenticatedClient(WebApplicationFactory<FornecedorController> factory)
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

    private sealed class DelegateHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handler;

        public DelegateHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _handler(request, cancellationToken);
        }
    }

    private sealed record ResponseEnvelope<T>(int Code, string? Message, T? Data);

    private sealed record CepData(string Cep, string Logradouro, string Bairro, string Cidade, string Estado);
}
