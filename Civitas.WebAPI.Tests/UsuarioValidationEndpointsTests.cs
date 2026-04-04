using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using BCrypt.Net;
using Civitas.WebAPI.Data;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace Civitas.WebAPI.Tests;

public sealed class UsuarioValidationEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly TestWebApplicationFactory _factory;

    public UsuarioValidationEndpointsTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostUsuario_WithMaskedFields_PersistsNormalizedValuesAndHashedPassword()
    {
        await _factory.ResetDatabaseAsync(_ => Task.CompletedTask);

        using var client = CreateAuthenticatedClient();
        var request = CreateUsuarioPayload(
            cpf: "529.982.247-25",
            rg: "12.345.678-X",
            cep: "87060-000",
            email: " USER@Example.COM ",
            senha: "abc12345",
            estado: "pr");

        var response = await client.PostAsJsonAsync("/api/usuarios", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var usuario = Assert.Single(context.Usuarios);

        Assert.Equal("52998224725", usuario.Cpf);
        Assert.Equal("12345678", usuario.Rg);
        Assert.Equal("87060000", usuario.Cep);
        Assert.Equal("user@example.com", usuario.Email);
        Assert.Equal("PR", usuario.Estado);
        Assert.NotEqual("abc12345", usuario.Senha);
        Assert.True(BCrypt.Net.BCrypt.Verify("abc12345", usuario.Senha));

        var body = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("\"senha\"", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PostUsuario_WithInvalidCpf_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync(_ => Task.CompletedTask);

        using var client = CreateAuthenticatedClient();
        var request = CreateUsuarioPayload(cpf: "111.111.111-11");

        var response = await client.PostAsJsonAsync("/api/usuarios", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("CPF", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PostUsuario_WithInvalidEmail_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync(_ => Task.CompletedTask);

        using var client = CreateAuthenticatedClient();
        var request = CreateUsuarioPayload(email: "email-invalido");

        var response = await client.PostAsJsonAsync("/api/usuarios", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("email", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PostUsuario_WithWeakPassword_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync(_ => Task.CompletedTask);

        using var client = CreateAuthenticatedClient();
        var request = CreateUsuarioPayload(senha: "abcdefghi");

        var response = await client.PostAsJsonAsync("/api/usuarios", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("senha", body, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("cpf")]
    [InlineData("email")]
    [InlineData("matricula")]
    public async Task PostUsuario_WithDuplicateUniqueField_ReturnsConflict(string duplicatedField)
    {
        await _factory.ResetDatabaseAsync(context =>
        {
            context.Usuarios.Add(CreateUsuario(
                id: 1,
                cpf: "52998224725",
                email: "duplicado@example.com",
                matricula: "MAT-001",
                senha: BCrypt.Net.BCrypt.HashPassword("abc12345")));

            return Task.CompletedTask;
        });

        using var client = CreateAuthenticatedClient();
        var request = CreateUsuarioPayload(
            cpf: duplicatedField == "cpf" ? "529.982.247-25" : "123.456.789-09",
            email: duplicatedField == "email" ? "DUPLICADO@example.com" : "novo@example.com",
            matricula: duplicatedField == "matricula" ? "MAT-001" : "MAT-999");

        var response = await client.PostAsJsonAsync("/api/usuarios", request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains(duplicatedField, body, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("XX", 1, 2)]
    [InlineData("PR", 3, 2)]
    [InlineData("PR", 1, 99)]
    public async Task PostUsuario_WithInvalidStateOrEnum_ReturnsBadRequest(string estado, int situacao, int tipoUsuario)
    {
        await _factory.ResetDatabaseAsync(_ => Task.CompletedTask);

        using var client = CreateAuthenticatedClient();
        var request = CreateUsuarioPayload(estado: estado);
        request["situacao"] = situacao;
        request["tipoUsuario"] = tipoUsuario;

        var response = await client.PostAsJsonAsync("/api/usuarios", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PutUsuario_WithoutPassword_PreservesCurrentHash()
    {
        const string currentHash = "$2a$11$7YNt1M0A6dwy1f7h8lJ4RO6vP9Hl9abZEWSIk2s9InBezVPSv+6mW";

        await _factory.ResetDatabaseAsync(context =>
        {
            context.Usuarios.Add(CreateUsuario(
                id: 1,
                cpf: "52998224725",
                email: "usuario@example.com",
                matricula: "MAT-001",
                senha: currentHash));

            return Task.CompletedTask;
        });

        using var client = CreateAuthenticatedClient();
        var request = CreateUsuarioPayload(
            cpf: "529.982.247-25",
            rg: "98.765.432-X",
            cep: "87060-000",
            email: "USUARIO@example.com",
            senha: null,
            matricula: "MAT-001");

        var response = await client.PutAsJsonAsync("/api/usuarios/1", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var usuario = await context.Usuarios.FindAsync(1);

        Assert.NotNull(usuario);
        Assert.Equal(currentHash, usuario!.Senha);
        Assert.Equal("98765432", usuario.Rg);
    }

    [Fact]
    public async Task PutUsuario_WithNewPassword_RehashesPassword()
    {
        var oldHash = BCrypt.Net.BCrypt.HashPassword("abc12345");

        await _factory.ResetDatabaseAsync(context =>
        {
            context.Usuarios.Add(CreateUsuario(
                id: 1,
                cpf: "52998224725",
                email: "usuario@example.com",
                matricula: "MAT-001",
                senha: oldHash));

            return Task.CompletedTask;
        });

        using var client = CreateAuthenticatedClient();
        var request = CreateUsuarioPayload(
            cpf: "529.982.247-25",
            email: "usuario@example.com",
            senha: "novaSenha123",
            matricula: "MAT-001");

        var response = await client.PutAsJsonAsync("/api/usuarios/1", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var usuario = await context.Usuarios.FindAsync(1);

        Assert.NotNull(usuario);
        Assert.NotEqual(oldHash, usuario!.Senha);
        Assert.True(BCrypt.Net.BCrypt.Verify("novaSenha123", usuario.Senha));
    }

    private static Dictionary<string, object?> CreateUsuarioPayload(
        string cpf = "123.456.789-09",
        string nome = "Usuario de Teste",
        string rg = "12.345.678-9",
        string logradouro = "Rua das Flores",
        string numero = "123",
        string bairro = "Centro",
        string cidade = "Maringa",
        string estado = "PR",
        string cep = "87060-000",
        string email = "usuario@example.com",
        string? senha = "abc12345",
        string matricula = "MAT-001",
        int tipoUsuario = (int)TipoUsuario.ADMINISTRADOR,
        int situacao = (int)Situacao.ATIVO)
    {
        return new Dictionary<string, object?>
        {
            ["cpf"] = cpf,
            ["nome"] = nome,
            ["rg"] = rg,
            ["logradouro"] = logradouro,
            ["numero"] = numero,
            ["bairro"] = bairro,
            ["cidade"] = cidade,
            ["estado"] = estado,
            ["cep"] = cep,
            ["email"] = email,
            ["senha"] = senha,
            ["matricula"] = matricula,
            ["tipoUsuario"] = tipoUsuario,
            ["situacao"] = situacao
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

    private static Usuario CreateUsuario(int id, string cpf, string email, string matricula, string senha)
    {
        return new Usuario(
            id,
            cpf,
            "Usuario Existente",
            "123456789",
            "Rua A",
            "10",
            "Maringa",
            "PR",
            "87060000",
            email,
            senha,
            Situacao.ATIVO,
            matricula,
            TipoUsuario.ADMINISTRADOR,
            "Centro");
    }
}
