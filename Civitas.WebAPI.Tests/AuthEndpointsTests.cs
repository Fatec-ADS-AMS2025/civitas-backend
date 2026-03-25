using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using BCrypt.Net;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Tests.Infrastructure;
using Xunit;

namespace Civitas.WebAPI.Tests;

public sealed class AuthEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly TestWebApplicationFactory _factory;

    public AuthEndpointsTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostLogin_WithValidCredentials_ReturnsJwtWithExpectedClaims()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            context.Usuarios.Add(CreateUsuario(
                id: 1,
                nome: "Maria da Silva",
                email: "maria@example.com",
                senha: BCrypt.Net.BCrypt.HashPassword("abc12345"),
                situacao: Situacao.ATIVO,
                tipoUsuario: TipoUsuario.ADMINISTRADOR));

            return Task.CompletedTask;
        });

        using var client = _factory.CreateClient();
        var request = new
        {
            email = " MARIA@example.com ",
            senha = "abc12345"
        };

        var beforeRequestUtc = DateTimeOffset.UtcNow;
        var response = await client.PostAsJsonAsync("/api/auth/login", request);
        var afterRequestUtc = DateTimeOffset.UtcNow;

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<LoginEnvelope>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal(1, body!.Code);
        Assert.NotNull(body.Data);
        Assert.False(string.IsNullOrWhiteSpace(body.Data!.Token));
        Assert.Equal(1, body.Data.Usuario.Id);
        Assert.Equal("Maria da Silva", body.Data.Usuario.Nome);
        Assert.Equal("ADMINISTRADOR", body.Data.Usuario.TipoUsuario);

        var jwtPayload = ReadJwtPayload(body.Data.Token);

        Assert.Equal("1", jwtPayload.Sub);
        Assert.Equal("Maria da Silva", jwtPayload.Name);
        Assert.Equal("ADMINISTRADOR", jwtPayload.TipoUsuario);

        var expiresAtUtc = DateTimeOffset.FromUnixTimeSeconds(jwtPayload.Exp);
        Assert.InRange(expiresAtUtc, beforeRequestUtc.AddMinutes(59), afterRequestUtc.AddMinutes(60));
    }

    [Fact]
    public async Task PostLogin_WithWrongPassword_ReturnsUnauthorized()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            context.Usuarios.Add(CreateUsuario(
                id: 1,
                nome: "Maria da Silva",
                email: "maria@example.com",
                senha: BCrypt.Net.BCrypt.HashPassword("abc12345"),
                situacao: Situacao.ATIVO,
                tipoUsuario: TipoUsuario.ADMINISTRADOR));

            return Task.CompletedTask;
        });

        using var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "maria@example.com",
            senha = "senhaErrada123"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ResponseEnvelope>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal(5, body!.Code);
    }

    [Fact]
    public async Task PostLogin_WithInactiveUser_ReturnsUnauthorized()
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            context.Usuarios.Add(CreateUsuario(
                id: 1,
                nome: "Maria da Silva",
                email: "maria@example.com",
                senha: BCrypt.Net.BCrypt.HashPassword("abc12345"),
                situacao: Situacao.INATIVO,
                tipoUsuario: TipoUsuario.ADMINISTRADOR));

            return Task.CompletedTask;
        });

        using var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "maria@example.com",
            senha = "abc12345"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("", "abc12345")]
    [InlineData("maria@example.com", "")]
    public async Task PostLogin_WithMissingFields_ReturnsBadRequest(string email, string senha)
    {
        await _factory.ResetDatabaseAsync(_ => Task.CompletedTask);

        using var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email,
            senha
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ResponseEnvelope>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal(2, body!.Code);
    }

    private static Usuario CreateUsuario(
        int id,
        string nome,
        string email,
        string senha,
        Situacao situacao,
        TipoUsuario tipoUsuario)
    {
        return new Usuario(
            id,
            "52998224725",
            nome,
            "12345678",
            "Rua das Flores",
            "123",
            "Maringa",
            "PR",
            "87060000",
            email,
            senha,
            situacao,
            "MAT-001",
            tipoUsuario,
            "Centro");
    }

    private static JwtPayload ReadJwtPayload(string token)
    {
        var segments = token.Split('.');
        Assert.Equal(3, segments.Length);

        var payloadBytes = DecodeBase64Url(segments[1]);
        return JsonSerializer.Deserialize<JwtPayload>(payloadBytes, JsonOptions)
            ?? throw new InvalidOperationException("Nao foi possivel ler o payload do JWT.");
    }

    private static byte[] DecodeBase64Url(string value)
    {
        var padded = value.Replace('-', '+').Replace('_', '/');
        padded += (padded.Length % 4) switch
        {
            2 => "==",
            3 => "=",
            _ => string.Empty
        };

        return Convert.FromBase64String(padded);
    }

    private sealed record ResponseEnvelope(int Code, string? Message, JsonElement? Data);

    private sealed record LoginEnvelope(int Code, string? Message, LoginData? Data);

    private sealed record LoginData(string Token, string ExpiresAtUtc, LoginUsuario Usuario);

    private sealed record LoginUsuario(int Id, string Nome, string TipoUsuario);

    private sealed record JwtPayload(string Sub, string Name, long Exp, string TipoUsuario);
}
